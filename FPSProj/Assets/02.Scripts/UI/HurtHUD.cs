using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtHUD : MonoBehaviour
{
    struct HurtData
    {
        public Transform shotOrigin;
        public Image hurtImage;
    }
    private Transform canvas;
    private GameObject hurtPrefab;

    private float decayFactor = 0.8f;

    private Dictionary<int, HurtData> hurtUIData;

    private Transform player, cam;

    public void Setup(Transform canvas, GameObject hurtPrefab, float decayFactor, Transform player)
    {
        hurtUIData = new Dictionary<int, HurtData>();
        this.canvas = canvas;
        this.hurtPrefab = hurtPrefab;
        this.decayFactor = decayFactor;
        this.player = player;

        cam = Camera.main.transform;
    }

    public void DrawHurtUI(Transform shotOrigin, int hashID)
    {
        if(hurtUIData.ContainsKey(hashID))
        {
            hurtUIData[hashID].hurtImage.color = GetUpdatedAlpha(hurtUIData[hashID].hurtImage.color, true);
        }
        else
        {
            GameObject hurtUI = Instantiate(hurtPrefab, canvas);
            SetRotation(hurtUI.GetComponent<Image>(), cam.forward, shotOrigin.position - player.position);

            HurtData data;
            data.shotOrigin = shotOrigin;
            data.hurtImage = hurtUI.GetComponent<Image>();
            hurtUIData.Add(hashID, data);
        }
    }

    private void SetRotation(Image hurtUI, Vector3 orientation, Vector3 shotDirection)
    {
        orientation.y = 0f;
        shotDirection.y = 0f;
        float rotation = Vector3.SignedAngle(shotDirection, orientation, Vector3.up);

        Vector3 newRotation = hurtUI.rectTransform.rotation.eulerAngles;
        newRotation.z = rotation;
        Image hurtImage = hurtUI.GetComponent<Image>();
        hurtImage.rectTransform.rotation = Quaternion.Euler(newRotation);
    }

    private Color GetUpdatedAlpha(Color currentColor, bool reset = false)
    {
        if(reset)
        {
            currentColor.a = 1f;
        }
        else
        {
            currentColor.a -= decayFactor * Time.deltaTime;
        }

        return currentColor;
    }
    private void Update()
    {
        List<int> toRemoveKeys = new List<int>();
        foreach (int key in hurtUIData.Keys)
        {
            SetRotation(hurtUIData[key].hurtImage, cam.forward, hurtUIData[key].shotOrigin.position - player.position);

            hurtUIData[key].hurtImage.color = GetUpdatedAlpha(hurtUIData[key].hurtImage.color);

            if (hurtUIData[key].hurtImage.color.a <= 0f)
            {
                toRemoveKeys.Add(key);
            }
        }

        for(int i =0; i < toRemoveKeys.Count; ++i)
        {
            // Destroy(hurtUIData[toRemoveKeys[i]].hurtImage.transform.gameObject);
            Destroy(hurtUIData[toRemoveKeys[i]].hurtImage.gameObject);
            hurtUIData.Remove(toRemoveKeys[i]);
        }
    }

}
