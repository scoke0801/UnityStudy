using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image FadeImage;
     
    // Start is called before the first frame update
    void Start()
    {
        // ���������� ���� �� ����, ���� ����
        StartCoroutine(FadeIn());

        // ���������� ���� �� ����, ���� ������ ������
        // StartCoroutine("FadeIn");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
        //    StopCoroutine("FadeIn");
        }
    }

    IEnumerator FadeIn()
    {

        Color startColor = FadeImage.color;

        for (int i = 0; i < 100; ++i)
        {
            startColor.a = startColor.a - 0.01f;
            FadeImage.color = startColor;
            yield return new WaitForSeconds(0.01f); 
        }
    }


}
