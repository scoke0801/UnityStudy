using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���⸦ ȹ���ϸ� ȹ���� ���⸦ UI�� ���� �����ְ�
/// ���� ��ź���� ��ü ������ �� �ִ� �Ѿ˷��� �����ش�.
/// </summary>
public class WeaponUIManager : MonoBehaviour
{
    public Color bulletColor = Color.white;
    public Color emptyBulletColor = Color.black;

    private Color noBulletColor; // �����ϰ� ���� ǥ��.

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

        // ������ �ȵ� �ּ�ó��.
        //int bulletCount = 0;
        //foreach(Transform bullet in bulletMag.transform)
        //{
        //    if(bulletCount < bulletLeft)
        //    {
        //        // ��ź
        //        bullet.GetComponent<Image>().color = bulletColor;
        //    }
        //    else if(bulletCount >= fullMag)
        //    {
        //        // �ʰ��� ź
        //        bullet.GetComponent<Image>().color = noBulletColor;
        //    }
        //    else
        //    {
        //        // ����� ź.
        //        bullet.GetComponent<Image>().color = emptyBulletColor;
        //    }

        //    ++bulletCount;
        //}
        totalBulletLabel.text = bulletLeft + "/" + extraBullets;
    }
}
