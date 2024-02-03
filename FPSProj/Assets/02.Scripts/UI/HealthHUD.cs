using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public float decayDuration = 2f;

    private Camera _camera;

    private Image hud, bar;

    private float decayTimer;
    private Color originalColor, noAlphaColor;

    private void Start()
    {
        hud = transform.Find("HUD").GetComponent<Image>();
        bar = transform.Find("Bar").GetComponent<Image>();

        _camera = Camera.main;

        originalColor = noAlphaColor = hud.color;

        noAlphaColor.a = 0f;

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if(gameObject.activeSelf == false)
        {
            return;
        }

        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
            _camera.transform.rotation * Vector3.up);
        decayTimer += Time.deltaTime;
        if(decayTimer >= 0.5f * decayDuration)
        {
            // 반정도 지났을 때.
            float from = decayTimer - (0.5f * decayDuration);
            float to = 0.5f * decayDuration;

            hud.color = Color.Lerp(originalColor, noAlphaColor, from / to);
            bar.color = Color.Lerp(originalColor, noAlphaColor, from / to);
        }

        if (decayTimer >= decayDuration)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetVisible()
    {
        gameObject.SetActive(true);
        decayTimer = 0f;
        hud.color = bar.color = originalColor;
    }
}
