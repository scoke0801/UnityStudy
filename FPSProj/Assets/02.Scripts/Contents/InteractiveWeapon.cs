using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �浹ü�� ������ ���⸦ ���� �� �ֵ��� �Ѵ�.
/// ���������� �浹ü�� ����.
/// ���⸦ �ٽ� �������� �־�� �ϸ�, �浹ü�� �ٽ� �ٿ��ش�.
/// �����ؼ� UI�� ��Ʈ�� �� �� �־�� �ϰ�
/// ShootBehavior�� ���� ���⸦ �־��ְ� �ȴ�.
/// </summary>
public class InteractiveWeapon : MonoBehaviour
{
    public string label_weaponName;     // ���� �̸�
    public SoundList shotSound, reloadSound, pickSound, dropSound, noBulletSound;
    public Sprite weaponSprite;
    
    public Vector3 rightHandPosition;   // �÷��̾� �����տ� ���� ��ġ.
    public Vector3 rightHandAimPosition;// �÷��̾� �����տ� ���� ��ġ(������).
    public Vector3 relativeRotation;    // �÷��̾� ���� ������ ���� ȸ�� ��.
    public float bulletDamage = 10f;
    public float recoilAngle;           // �ݵ�

    public enum WeaponType
    {
        NONE,
        SHORT,
        LONG,

    }

    public enum WeaponMode
    {
        SEMI,  
        BURST,
        AUTO,
    }

    public WeaponType weaponType = WeaponType.NONE;
    public WeaponMode weaponMode = WeaponMode.SEMI;
    public int burstSize = 1;

    public int currentMagCapacity; // ���� źâ ��
    public int totalBullet;         // �����ϰ� �ִ� ��ü �Ѿ� ��
    private int fullMag;            // ������ �� �� ä��� źâ
    private int maxBullet;          // �� ���� ä�� �� �ִ� �ִ� �Ѿ�

    private GameObject player;
    private GameObject gameController;
    private ShootBehavior playerInventory;  // �κ��丮�� �ϴ� ���⿡ ����...

    private BoxCollider weaponCollider;
    private SphereCollider intertactiveRadius;
    private Rigidbody weaponRigidbody;
    private bool pickable;

    // UI
    public GameObject screenHUD;
    public WeaponUIManager weaponHUD;
    private Transform pickHUD;
    public Text pickupHUD_Label;

    public Transform muzzleTransform;

    private void Awake()
    {
        gameObject.name = label_weaponName;
        gameObject.layer = LayerMask.NameToLayer(Defs.TagAndLayer.LayerName.IgnoreRayCast);

        foreach(Transform tr in transform)
        {
            tr.gameObject.layer = LayerMask.NameToLayer(Defs.TagAndLayer.LayerName.IgnoreRayCast);
        }

        player = GameObject.FindGameObjectWithTag(Defs.TagAndLayer.TagName.Player);

        playerInventory = player.GetComponent<ShootBehavior>();
        gameController = GameObject.FindGameObjectWithTag(Defs.TagAndLayer.TagName.GameController);

        if(weaponHUD == null)
        {
            if (screenHUD == null)
            {
                screenHUD = GameObject.Find("ScreenHUD");
            }

            weaponHUD = screenHUD.GetComponent<WeaponUIManager>();
        }

        if (pickHUD == null)
        {
            pickHUD = gameController.transform.Find("PickupHUD");
        }

        // ���ͷ����� ���� �浹ü ����.
        weaponCollider = transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        CreateInteractiveRadius(weaponCollider.center);

        weaponRigidbody = gameObject.AddComponent<Rigidbody>();

        if(weaponType == WeaponType.NONE)
        {
            weaponType = WeaponType.SHORT;
        }

        fullMag = currentMagCapacity;
        maxBullet = totalBullet;
        pickHUD.gameObject.SetActive(false);

        if(muzzleTransform == null)
        {
            muzzleTransform = transform.Find("muzzle");
        }
    }

    private void CreateInteractiveRadius(Vector3 center)
    {
        intertactiveRadius = gameObject.AddComponent<SphereCollider>();

        intertactiveRadius.center = center;
        intertactiveRadius.radius = 1;
        intertactiveRadius.isTrigger = true;
    }

    private void TogglePickHUD(bool toggle)
    {
        pickHUD.gameObject.SetActive(toggle);
        if (toggle)
        {
            pickHUD.position = transform.position + Vector3.up * 0.5f;
            Vector3 direction = player.GetComponent<BehaviorController>().playerCamera.forward;
            direction.y = 0f;

            // UI�� ī�޶� �ٶ󺸰�...
            pickHUD.rotation = Quaternion.LookRotation(direction);
            pickupHUD_Label.text = "Pick " + gameObject.name;
        }
    }

    private void UpdateHUD()
    {
        weaponHUD.UpdateWeaponHUD(weaponSprite, currentMagCapacity, fullMag, totalBullet);
    }

    public void Toggle(bool active)
    {
        if(active)
        {
            SoundManager.Instance.PlayOneShotEffect((int)pickSound, transform.position, 0.5f);
        }

        weaponHUD.Toggle(active);
        UpdateHUD();
    }

    private void Update()
    {
        // �������� �������� ���� ó��.
        if(pickable && Input.GetButtonDown(ButtonName.Pick))
        {
            // disable physics weapon
            weaponRigidbody.isKinematic = true;
            weaponCollider.enabled = false;
            playerInventory.AddWeapon(this);

            Destroy(intertactiveRadius);

            Toggle(true);
            pickable = false;

            TogglePickHUD(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject != player &&
            Vector3.Distance(transform.position, player.transform.position) <= 5f)
        {
            SoundManager.Instance.PlayOneShotEffect((int)dropSound, transform.position, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            pickable = false;
            TogglePickHUD(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player && 
            playerInventory &&
            playerInventory.isActiveAndEnabled)
        {
            pickable = true;
            TogglePickHUD(true);
        }
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        transform.position += Vector3.up;
        weaponRigidbody.isKinematic = false;
        transform.parent = null;
        CreateInteractiveRadius(weaponCollider.center);
        weaponCollider.enabled = true;
        weaponHUD.Toggle(false);
    }

    public bool StartReload()
    {
        if(currentMagCapacity == fullMag || totalBullet == 0)
        {
            return false;
        }
        else if(totalBullet < fullMag - currentMagCapacity)
        {
            currentMagCapacity += totalBullet;
            totalBullet = 0;
        }
        else
        {
            totalBullet -= fullMag - currentMagCapacity;
            currentMagCapacity = fullMag;
        }

        return true;
    }

    public void EndRelaod()
    {
        UpdateHUD();
    }

    public bool Shoot(bool firstShot = true)
    {
        if(currentMagCapacity > 0)
        {
            currentMagCapacity--;
            UpdateHUD();
            return true;
        }
        
        if(firstShot && noBulletSound != SoundList.None)
        {
            SoundManager.Instance.PlayOneShotEffect((int)noBulletSound, muzzleTransform.position, 5f);
        }

        return false;
    }

    public void ResetBullet()
    {
        currentMagCapacity = fullMag;
        totalBullet = maxBullet;
    }
}
