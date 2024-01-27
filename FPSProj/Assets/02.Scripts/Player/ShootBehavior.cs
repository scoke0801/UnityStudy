using Defs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ���: ����� �������� ���θ� üũ�ϴ� ���.
/// �߻� Ű �Է� �޾Ƽ� �ִϸ��̼� ���, ����Ʈ ����, �浹 üũ ���.
/// UI �����ؼ� ���ڼ� ǥ�� ���
/// �߻� �ӵ� ����
/// ĳ���� ��ü�� IK�� �̿��ؼ� ���� ������ ���缭 ȸ��.
/// ���̳� �浹ü�� �Ѿ��� �ǰݵǾ��� ��� ��ź ����Ʈ�� ����.
/// �κ��丮 ����. ���⸦ �����ϰ� �ִ� �� Ȯ�ο�.
/// �������� ���� ��ü ���
/// </summary>
public class ShootBehavior : GenericBehavior
{
    public Texture2D aimCrossHair, shootCrossHair;
    public GameObject muzzleFlash, shot, sparks;
    public Material bulletHole;
    public int MaxBulletHoles = 50;
    public float shootErrorRat = 0.01f; // ���� Ȯ��..
    public float shootRateFactor = 1f;  // �߻� �ӵ�.
    public float armsRotation = 8f;     // ���ؽ� �� ȸ��.

    public LayerMask shotMask = ~(TagAndLayer.LayerMasking.IgnoreRayCast | TagAndLayer.LayerMasking.IgnoreShot |
        TagAndLayer.LayerMasking.CoverInvisible | TagAndLayer.LayerMasking.Player);

    // ����ü�� Ȯ���ϱ� ���ؼ�.
    public LayerMask organicMask = TagAndLayer.LayerMasking.Player | TagAndLayer.LayerMasking.Enemy;

    // ª�� ��, �ǽ��� ���� ���� ����� �� ���� �� ���� ��ġ�� �����ϱ� ��gka
    public Vector3 leftArmShortAIm = new Vector3(-4.0f, 0.0f, 2.0f);

    private int activeWeapon = 0;

    // �ִϸ��̼ǿ� ��
    private int weaponType;
    private int changeWeaponTrigger;
    private int shootingTrigger;
    private int aimBool;
    private int blockedAimBool;
    private int reloadBool;

    private bool isAiming;
    private bool isAimBlocked;
    private bool isShooting = false;
    private bool isChangingWeapon = false;
    private bool isBulletAlive = false;

    private List<InteractiveWeapon> weapons; // �����ϰ� �ִ� �����.

    private Transform gunMuzzle;
    private float distToHand;

    private Vector3 castRelativeOrigin;

    // ���� Ÿ�Կ� ���� ��ġ�� ����.
    private Dictionary<InteractiveWeapon.WeaponType, int> slotDict;

    private Transform hips, spine, chest, rightHand, leftArm;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;

    private float shotInterval;                     // �Ѿ� ����
    private float originalShotInterval = 0.5f;      // �Ѿ� ����

    private List<GameObject> bulletHoles;
    private int bulletHoleSlot = 0;
    private int burstShotCount = 0;
    private AimBehavior aimBehavior;
    private Texture2D originalCrossHair;

    private void Start()
    {
        weaponType = Animator.StringToHash(AnimatorKey.Weapon);
        aimBool = Animator.StringToHash(AnimatorKey.Aim);
        blockedAimBool = Animator.StringToHash(AnimatorKey.BlockedAim);
        changeWeaponTrigger = Animator.StringToHash(AnimatorKey.ChangeWeapon);
        shootingTrigger = Animator.StringToHash(AnimatorKey.Shooting);
        reloadBool = Animator.StringToHash(AnimatorKey.Reload);
        weapons = new List<InteractiveWeapon>(new InteractiveWeapon[3]);
        aimBehavior = GetComponent<AimBehavior>();
        bulletHoles = new List<GameObject>();

        muzzleFlash.SetActive(false);
        shot.SetActive(false);
        sparks.SetActive(false);

        slotDict = new Dictionary<InteractiveWeapon.WeaponType, int>
        {
            { InteractiveWeapon.WeaponType.SHORT, 1 },
            { InteractiveWeapon.WeaponType.LONG, 2},
        };

        Transform neck = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Neck);
        if(!neck)
        {
            neck = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Head).parent;
        }
        hips = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        spine = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
        chest = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Chest);
        rightHand = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        leftArm = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;
        initialChestRotation = chest.localEulerAngles;

        originalCrossHair = aimBehavior.crossHair;
        shotInterval = originalShotInterval;
        castRelativeOrigin = neck.position - transform.position;
        distToHand = (rightHand.position - neck.position).magnitude * 1.5f;
    }

    private void DrawShoot(GameObject weapon, Vector3 destination, Vector3 targetNormal,Transform parent,
        bool placeSparks = true, bool placeBulletHole = true)
    {
        Vector3 origin = gunMuzzle.position - gunMuzzle.right * 0.5f;

        muzzleFlash.SetActive(true);
        muzzleFlash.transform.SetParent(gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localEulerAngles = Vector3.back * 90f;

        // Ǯ���ϴ°� ���� �ҵ�..
        GameObject instanceShot = EffectManager.Instance.EffectOneShot((int)EffectList.Tracer, origin);
        instanceShot.SetActive(true);
        instanceShot.transform.rotation = Quaternion.LookRotation(destination - origin);
        instanceShot.transform.parent = shot.transform.parent;


        if(placeSparks)
        {
            GameObject instaceSpark = EffectManager.Instance.EffectOneShot((int)EffectList.Sparks, destination);
            instaceSpark.SetActive(true);
            instaceSpark.transform.SetParent(sparks.transform.parent);
        }

        if(placeBulletHole)
        {
            Quaternion hitRoation = Quaternion.FromToRotation(Vector3.back, targetNormal);
            GameObject bullet = null;
            if(bulletHoles.Count < MaxBulletHoles)
            {
                bullet = GameObject.CreatePrimitive(PrimitiveType.Quad);
                bullet.GetComponent<MeshRenderer>().material = bulletHole;
                bullet.GetComponent<Collider>().enabled = false;
                bullet.transform.localScale = Vector3.one * 0.07f;
                bullet.name = "BulletHole";
                bulletHoles.Add(bullet);
            }
            else
            {
                bullet = bulletHoles[bulletHoleSlot];
                bulletHoleSlot++;
                bulletHoleSlot %= MaxBulletHoles;
            }

            // ��ġ�� ��¦ �ٸ��� ��ġ���������� ���ļ� �ڱ��ڱ��ϰ� ����.
            bullet.transform.position = destination + 0.01f * targetNormal;
            bullet.transform.rotation = hitRoation;
            bullet.transform.SetParent(parent);
        }
    }

    private void ShootWeapon(int weapon, bool firstShot = true)
    {
        if(!isAiming || isAimBlocked || behaviorController.GetAnimator.GetBool(reloadBool) ||
            !weapons[weapon].Shoot(firstShot))
        {
            // ���� ���� �ƴϰų�
            // ������ �����ų�
            // ������ ���̰ų�
            // 
            return;
        }
        else
        {
            burstShotCount++;
            behaviorController.GetAnimator.SetTrigger(shootingTrigger);
            aimBehavior.crossHair = shootCrossHair;

            // �ݵ� �ֱ�.
            behaviorController.GetCamScript.BounceVertical(weapons[weapon].recoilAngle);

            // ������...
            Vector3 imprecision = Random.Range(-shootErrorRat, shootErrorRat) *
                behaviorController.playerCamera.forward;
            Ray ray = new Ray(behaviorController.playerCamera.position,
                behaviorController.playerCamera.forward + imprecision);

            RaycastHit hit = default(RaycastHit);
            if(Physics.Raycast(ray, out hit, 500f, shotMask))
            {
                if(hit.collider.transform != transform)
                {
                    // ����ü�� ������ �ʵ���.
                    bool isOrganic = (organicMask == (organicMask | 1 << hit.transform.root.gameObject.layer));

                    DrawShoot(weapons[weapon].gameObject, hit.point, hit.normal, hit.collider.transform, !isOrganic, !isOrganic);
                
                    if(hit.collider)
                    {
                        hit.collider.SendMessageUpwards("HitCallback", new HealthBase.DamageInfo(
                            hit.point, ray.direction, weapons[weapon].bulletDamage,hit.collider),
                            SendMessageOptions.DontRequireReceiver);
                    }
                }
                else
                {
                    Vector3 destination = (ray.direction * 500f) - ray.origin;
                    DrawShoot(weapons[weapon].gameObject, destination, Vector3.up, null, false, false);
                }

                SoundManager.Instance.PlayOneShotEffect((int)weapons[weapon].shotSound, gunMuzzle.position, 5f);
                GameObject gameController = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.GameController);
                gameController.SendMessage("RootAlertNearBy", ray.origin, SendMessageOptions.DontRequireReceiver);

                shotInterval = originalShotInterval;

                isBulletAlive = true;
            }
        }
    }

    public void EndReloadWeapon()
    {
        behaviorController.GetAnimator.SetBool(reloadBool, false);
        weapons[activeWeapon].EndRelaod();
    }

    private void SetWeaponCrossHair(bool armed)
    {
        if (armed)
        {
            aimBehavior.crossHair = aimCrossHair;
        }
        else
        {
            aimBehavior.crossHair = originalCrossHair;
        }
    }

    private void ShotProgress()
    {
        if(shotInterval > 0.2f)
        {
            shotInterval -= shootRateFactor * Time.deltaTime;
            if(shotInterval <= 0.5f)
            {
                SetWeaponCrossHair(activeWeapon > 0);
                muzzleFlash.SetActive(false);
                if(activeWeapon > 0)
                {
                    behaviorController.GetCamScript.BounceVertical(-weapons[activeWeapon].recoilAngle * 0.1f);

                    if(shotInterval <=  (0.5f - 2f * Time.deltaTime))
                    {
                        if (weapons[activeWeapon].weaponMode == InteractiveWeapon.WeaponMode.AUTO &&
                            Input.GetAxisRaw(ButtonName.Shoot) != 0)
                        {
                            ShootWeapon(activeWeapon, false);
                        }
                        else if (weapons[activeWeapon].weaponMode == InteractiveWeapon.WeaponMode.BURST &&
                            burstShotCount < weapons[activeWeapon].burstSize)
                        {
                            ShootWeapon(activeWeapon, false);
                        }
                        else if (weapons[activeWeapon].weaponMode != InteractiveWeapon.WeaponMode.BURST)
                        {
                            burstShotCount = 0;
                        }
                    }
                }
            }
        }
        else
        {
            isBulletAlive = false;
            behaviorController.GetCamScript.BounceVertical(0);
            burstShotCount = 0;
        }
    }

    private void ChangeWeapon(int oldWeapon, int newWeapon)
    { 
        if(oldWeapon > 0)
        {
            weapons[oldWeapon].gameObject.SetActive(false);
            gunMuzzle = null;
            weapons[oldWeapon].Toggle(false);
        }

        // �� ���� ã��
        while (weapons[newWeapon] == null && newWeapon > 0)
        {
            newWeapon = (newWeapon + 1) % weapons.Count;
        }

        if(newWeapon > 0)
        {
            weapons[newWeapon].gameObject.SetActive(true);
            gunMuzzle = weapons[newWeapon].transform.Find("muzzle");

            weapons[newWeapon].Toggle(true);
        }

        activeWeapon = newWeapon;
        if(oldWeapon != newWeapon)
        {
            behaviorController.GetAnimator.SetTrigger(changeWeaponTrigger);
            behaviorController.GetAnimator.SetInteger(weaponType, weapons[newWeapon] ?
                (int)weapons[newWeapon].weaponType : 0);
        }

        SetWeaponCrossHair(newWeapon > 0);
    }

    private void Update()
    {
        float shootTrigger = Mathf.Abs(Input.GetAxisRaw(ButtonName.Shoot));
        if(shootTrigger > Mathf.Epsilon && !isShooting && activeWeapon > 0 && burstShotCount > 0)
        {
            Debug.Log("SHootStartPlz");
            // ��� ����.
            isShooting = true;
            ShootWeapon(activeWeapon);
        }
        else if(isShooting && shootTrigger < Mathf.Epsilon)
        {
            Debug.Log("SHootEndPlz");
            // ��� ��.
            isShooting = false;
        }
        else if (Input.GetButtonUp(ButtonName.Reload) && activeWeapon > 0)
        {
            // ������.
            if (weapons[activeWeapon].StartReload())
            {
                SoundManager.Instance.PlayOneShotEffect((int)weapons[activeWeapon].reloadSound,
                    gunMuzzle.position, 0.5f);
                behaviorController.GetAnimator.SetBool(reloadBool, true);
            }
        }
        else if(Input.GetButtonDown(ButtonName.Drop) && activeWeapon > 0)
        {
            // ���� ���.
            EndReloadWeapon();
            int weaponToDrop = activeWeapon;
            ChangeWeapon(activeWeapon, 0);
            weapons[weaponToDrop].Drop();
            weapons[weaponToDrop] = null;
        }
        else
        { 
            if(Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) > Mathf.Epsilon && !isChangingWeapon)
            {
                // ���� ��ü ����.
                isChangingWeapon = true;
                int nextWeapon = activeWeapon + 1;
                ChangeWeapon(activeWeapon, nextWeapon % weapons.Count);
            }
            else if (Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) < Mathf.Epsilon)
            {
                // ���� ��ü ��.
                isChangingWeapon = false;
            }
        }

        if (isShooting)
        {
            ShotProgress();
        }

        isAiming = behaviorController.GetAnimator.GetBool(aimBool);
    }





    /// <summary>
    /// �κ��丮 ������ �ϰԵ� �Լ�.
    /// </summary>
    /// 
    public void AddWeapon(InteractiveWeapon newWeapon)
    {
        newWeapon.gameObject.transform.SetParent(rightHand);
        newWeapon.transform.localPosition = newWeapon.rightHandPosition;
        newWeapon.transform.localRotation = Quaternion.Euler(newWeapon.relativeRotation);

        if (weapons[slotDict[newWeapon.weaponType]])
        {
            // �̹� ������ �ִ� ������
            if (weapons[slotDict[newWeapon.weaponType]].label_weaponName == newWeapon.label_weaponName)
            {
                // �̸����� ���ٸ�, �Ѿ� ä���ֱ�.
                weapons[slotDict[newWeapon.weaponType]].ResetBullet();
                ChangeWeapon(activeWeapon, slotDict[newWeapon.weaponType]);
                Destroy(newWeapon.gameObject);
                return;
            }
            else
            {
                // �̸��� �ٸ� ���. ������ �ִ� ����� ��� ó��.
                weapons[slotDict[newWeapon.weaponType]].Drop();
            }
        }

        weapons[slotDict[newWeapon.weaponType]] = newWeapon;
        ChangeWeapon(activeWeapon, slotDict[newWeapon.weaponType]);
    }

    private bool CheckForBlockedAim()
    {
        isAimBlocked = Physics.SphereCast(transform.position + castRelativeOrigin, 0.1f,
            behaviorController.GetCamScript.transform.forward, out RaycastHit hit, distToHand - 0.1f);
        isAimBlocked = isAimBlocked && hit.collider.transform != transform;
        behaviorController.GetAnimator.SetBool(blockedAimBool, isAimBlocked);
        Debug.DrawRay(transform.position + castRelativeOrigin,
            behaviorController.GetCamScript.transform.forward * distToHand, isAimBlocked ? Color.red : Color.green);

        return isAimBlocked;
    }

    public void OnAnimatorIK(int layerIndex)
    {
        if(isAiming && activeWeapon > 0)
        {
            if (CheckForBlockedAim())
            {
                return;
            }

            Quaternion targetRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            targetRot *= Quaternion.Euler(initialRootRotation);
            targetRot *= Quaternion.Euler(initialHipsRotation);
            targetRot *= Quaternion.Euler(initialSpineRotation);
            behaviorController.GetAnimator.SetBoneLocalRotation(HumanBodyBones.Spine,
                Quaternion.Inverse(hips.rotation) * targetRot);

            float xcamRot = Quaternion.LookRotation(behaviorController.playerCamera.forward).eulerAngles.x;
            targetRot = Quaternion.AngleAxis(xcamRot + armsRotation, transform.right);
            if (weapons[activeWeapon] && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.LONG)
            {
                targetRot *= Quaternion.AngleAxis(9f, transform.right);
                targetRot *= Quaternion.AngleAxis(20f, transform.up);
            }

            targetRot *= spine.rotation;
            targetRot *= Quaternion.Euler(initialChestRotation);
            behaviorController.GetAnimator.SetBoneLocalRotation(HumanBodyBones.Chest,
                Quaternion.Inverse(spine.rotation) * targetRot);
        }
    }

    private void LateUpdate()
    {
        // ���� ���̰�, �������� ���Ⱑ ���� ������
        if (isAiming && weapons[activeWeapon]
            && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.SHORT)
        {
            leftArm.localEulerAngles = leftArm.localEulerAngles + leftArmShortAIm;
        }
    }
}
