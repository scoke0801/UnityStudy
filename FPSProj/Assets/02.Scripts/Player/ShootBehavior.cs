using Defs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사격 기능: 사격이 가능한지 여부를 체크하는 기능.
/// 발사 키 입력 받아서 애니메이션 재생, 이펙트 생성, 충돌 체크 기능.
/// UI 관련해서 십자선 표시 기능
/// 발사 속도 조정
/// 캐릭터 상체를 IK를 이용해서 조준 시점에 맞춰서 회전.
/// 벽이나 충돌체에 총알이 피격되었을 경우 피탄 이펙트를 생성.
/// 인벤토리 역할. 무기를 소지하고 있는 지 확인용.
/// 재장전과 무기 교체 기능
/// </summary>
public class ShootBehavior : GenericBehavior
{
    public Texture2D aimCrossHair, shootCrossHair;
    public GameObject muzzleFlash, shot, sparks;
    public Material bulletHole;
    public int MaxBulletHoles = 50;
    public float shootErrorRat = 0.01f; // 오발 확률..
    public float shootRateFactor = 1f;  // 발사 속도.
    public float armsRotation = 8f;     // 조준시 팔 회전.

    public LayerMask shotMask = ~(TagAndLayer.LayerMasking.IgnoreRayCast | TagAndLayer.LayerMasking.IgnoreShot |
        TagAndLayer.LayerMasking.CoverInvisible | TagAndLayer.LayerMasking.Player);

    // 생명체를 확인하기 위해서.
    public LayerMask organicMask = TagAndLayer.LayerMasking.Player | TagAndLayer.LayerMasking.Enemy;

    // 짧은 총, 피스톨 같은 총을 들었을 때 조준 시 팔의 위치를 보정하기 위gka
    public Vector3 leftArmShortAIm = new Vector3(-4.0f, 0.0f, 2.0f);

    private int activeWeapon = 0;

    // 애니메이션용 값
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

    private List<InteractiveWeapon> weapons; // 소지하고 있는 무기들.

    private Transform gunMuzzle;
    private float distToHand;

    private Vector3 castRelativeOrigin;

    // 무기 타입에 따라 위치할 슬롯.
    private Dictionary<InteractiveWeapon.WeaponType, int> slotDict;

    private Transform hips, spine, chest, rightHand, leftArm;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;

    private float shotInterval;                     // 총알 수명
    private float originalShotInterval = 0.5f;      // 총알 수명

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

        // 풀링하는게 좋긴 할듯..
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

            // 위치를 살짝 다르게 배치하지않으면 겹쳐서 자글자글하게 보임.
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
            // 조준 중이 아니거나
            // 조준이 막혔거나
            // 재장전 중이거나
            // 
            return;
        }
        else
        {
            burstShotCount++;
            behaviorController.GetAnimator.SetTrigger(shootingTrigger);
            aimBehavior.crossHair = shootCrossHair;

            // 반동 주기.
            behaviorController.GetCamScript.BounceVertical(weapons[weapon].recoilAngle);

            // 실패율...
            Vector3 imprecision = Random.Range(-shootErrorRat, shootErrorRat) *
                behaviorController.playerCamera.forward;
            Ray ray = new Ray(behaviorController.playerCamera.position,
                behaviorController.playerCamera.forward + imprecision);

            RaycastHit hit = default(RaycastHit);
            if(Physics.Raycast(ray, out hit, 500f, shotMask))
            {
                if(hit.collider.transform != transform)
                {
                    // 생명체에 생기지 않도록.
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

        // 빈 슬롯 찾기
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
            // 사격 시작.
            isShooting = true;
            ShootWeapon(activeWeapon);
        }
        else if(isShooting && shootTrigger < Mathf.Epsilon)
        {
            Debug.Log("SHootEndPlz");
            // 사격 끝.
            isShooting = false;
        }
        else if (Input.GetButtonUp(ButtonName.Reload) && activeWeapon > 0)
        {
            // 재장전.
            if (weapons[activeWeapon].StartReload())
            {
                SoundManager.Instance.PlayOneShotEffect((int)weapons[activeWeapon].reloadSound,
                    gunMuzzle.position, 0.5f);
                behaviorController.GetAnimator.SetBool(reloadBool, true);
            }
        }
        else if(Input.GetButtonDown(ButtonName.Drop) && activeWeapon > 0)
        {
            // 무기 드랍.
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
                // 무기 교체 시작.
                isChangingWeapon = true;
                int nextWeapon = activeWeapon + 1;
                ChangeWeapon(activeWeapon, nextWeapon % weapons.Count);
            }
            else if (Mathf.Abs(Input.GetAxisRaw(ButtonName.Change)) < Mathf.Epsilon)
            {
                // 무기 교체 끝.
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
    /// 인벤토리 역할을 하게될 함수.
    /// </summary>
    /// 
    public void AddWeapon(InteractiveWeapon newWeapon)
    {
        newWeapon.gameObject.transform.SetParent(rightHand);
        newWeapon.transform.localPosition = newWeapon.rightHandPosition;
        newWeapon.transform.localRotation = Quaternion.Euler(newWeapon.relativeRotation);

        if (weapons[slotDict[newWeapon.weaponType]])
        {
            // 이미 가지고 있는 무기라면
            if (weapons[slotDict[newWeapon.weaponType]].label_weaponName == newWeapon.label_weaponName)
            {
                // 이름까지 같다면, 총알 채워주기.
                weapons[slotDict[newWeapon.weaponType]].ResetBullet();
                ChangeWeapon(activeWeapon, slotDict[newWeapon.weaponType]);
                Destroy(newWeapon.gameObject);
                return;
            }
            else
            {
                // 이름은 다른 경우. 가지고 있던 무기는 드랍 처리.
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
        // 조준 중이고, 착용중인 무기가 작은 무기라면
        if (isAiming && weapons[activeWeapon]
            && weapons[activeWeapon].weaponType == InteractiveWeapon.WeaponType.SHORT)
        {
            leftArm.localEulerAngles = leftArm.localEulerAngles + leftArmShortAIm;
        }
    }
}
