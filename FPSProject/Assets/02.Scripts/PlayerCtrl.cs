using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private float _moveSpeed = 1.5f;
    private float _turnSpeed = 90.0f;

    private Transform tr;
    private Animation anim;

    private readonly float initHp = 100.0f;
    public float currHp;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    private Image hpBar;

    private IEnumerator Start()
    {
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        currHp = initHp;
        DisplayHealth();
     
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        anim.Play("Idle");

        // For Test.
        _turnSpeed = 0.0f;
        yield return new WaitForSeconds(0);
        _turnSpeed = 80.0f;

    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");  // -1.0f ~ 0.0f ~ 1.0f
        float v = Input.GetAxis("Vertical");    // -1.0f ~ 0.0f ~ 1.0f
        float r = Input.GetAxis("Mouse X");

        tr.Translate(Vector3.forward * _moveSpeed * Time.deltaTime * v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * _moveSpeed * Time.deltaTime);

        tr.Rotate(Vector3.up * Time.deltaTime * _turnSpeed * r);

        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v)
    {
        if(v>= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);
        }
        else if( v<= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (h > 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if(h< -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }

    private void OnTriggerEnter(Collider coll )
    {
        if (currHp > 0.0f && coll.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;
            DisplayHealth();

            Debug.Log($"PlayerHp = {currHp / initHp}");

            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die!");

        OnPlayerDie();

        GameManager.instance.IsGameOver = true;
    }

    void DisplayHealth()
    {
        hpBar.fillAmount = currHp / initHp;
    }
}
