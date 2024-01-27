using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기를 획득하면 획득한 무기를 UI를 통해 보여주고
/// 현재 잔탄량과 전체 소지할 수 있는 총알량을 보여준다.
/// </summary>
public class WeaponUIManager : MonoBehaviour
{
    public Color bulletColor = Color.white;
    public Color emptyBulletColor = Color.black;

    private Color noBulletColor; // 투명하게 색깔 표시.

    [SerializeField] private Image weaponHUD;
    [SerializeField] private GameObject bulletMag;
    [SerializeField] private Text totalBulletLabel;
    

    void Start()
    {
        noBulletColor = new Color(0f, 0f, 0f, 0f);
        
        if(weaponHUD == null)
        {
            weaponHUD = transform.Find("WeaponHUD/weapon").GetComponent<Image>();
        }

        if(bulletMag == null)
        {
            bulletMag = transform.Find("WeapongHUD/Data/Mag").gameObject;
        }

        if(totalBulletLabel == null)
        {
            totalBulletLabel = transform.Find("WeaponHUD/Data/bulletAmount").GetComponent<Text>();
        }

        Toggle(false);
    }

    public void Toggle(bool active)
    {
        weaponHUD.transform.parent.gameObject.SetActive(active);
    }
    public void UpdateWeaponHUD(Sprite weaponSprite, int bulletLeft, int fullMag, int extraBullets)
    {
        if(weaponSprite != null && weaponHUD.sprite != weaponSprite)
        {
            weaponHUD.sprite = weaponSprite;
            weaponHUD.type = Image.Type.Filled;
            weaponHUD.fillMethod = Image.FillMethod.Horizontal;
        }

        // 마음에 안들어서 주석처리.
        //int bulletCount = 0;
        //foreach(Transform bullet in bulletMag.transform)
        //{
        //    if(bulletCount < bulletLeft)
        //    {
        //        // 잔탄
        //        bullet.GetComponent<Image>().color = bulletColor;
        //    }
        //    else if(bulletCount >= fullMag)
        //    {
        //        // 초과된 탄
        //        bullet.GetComponent<Image>().color = noBulletColor;
        //    }
        //    else
        //    {
        //        // 사용한 탄.
        //        bullet.GetComponent<Image>().color = emptyBulletColor;
        //    }

        //    ++bulletCount;
        //}
        totalBulletLabel.text = bulletLeft + "/" + extraBullets;
    }
}
