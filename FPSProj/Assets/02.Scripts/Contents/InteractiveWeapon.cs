using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 충돌체를 생성해 무기를 주을 수 있도록 한다.
/// 루팅했으면 충돌체는 제거.
/// 무기를 다시 버릴수도 있어야 하며, 충돌체를 다시 붙여준다.
/// 관려해서 UI도 컨트롤 할 수 있어야 하고
/// ShootBehavior에 줏은 무기를 넣어주게 된다.
/// </summary>
public class InteractiveWeapon : MonoBehaviour
{
    public string label_weaponName;     // 무기 이름
    public SoundList shotSound, reloadSound, pickSound, dropSound, noBulletSound;
    public Sprite weaponSprite;
    
    public Vector3 rightHandPosition;   // 플레이어 오른손에 고정 위치.
    public Vector3 rightHandAimPosition;// 플레이어 오른손에 고정 위치(조준중).
    public Vector3 relativeRotation;    // 플레이어 맞춘 보정을 위한 회전 값.
    public float bulletDamage = 10f;
    public float recoilAngle;           // 반동

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

    public int currentMagCapacity; // 현재 탄창 양
    public int totalBullet;         // 소지하고 있는 전체 총알 양
    private int fullMag;            // 재장전 시 꽉 채우는 탄창
    private int maxBullet;          // 한 번에 채울 수 있는 최대 총알

    private GameObject player;
    private GameObject gameController;
    private ShootBehavior playerInventory;  // 인벤토리를 일단 여기에 구현...

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

        // 인터렉션을 위한 충돌체 설정.
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

            // UI가 카메라를 바라보게...
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
        // 아이템을 습득했을 때의 처리.
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
