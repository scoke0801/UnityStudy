using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    #region Variables
    private TextMeshProUGUI _textMeshPro;

    [SerializeField] private float _delayTimeToDestroy = 1.0f;
    #endregion

    #region 
    public int Damage
    {
        get
        {
            if(_textMeshPro != null)
            {
                return int.Parse(_textMeshPro.text);
            }

            return 0;
        }
        set
        {
            if( _textMeshPro != null)
            {
                _textMeshPro.text = value.ToString();
            }
        }
    }
    #endregion

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Destroy(gameObject, _delayTimeToDestroy);
    }
}
