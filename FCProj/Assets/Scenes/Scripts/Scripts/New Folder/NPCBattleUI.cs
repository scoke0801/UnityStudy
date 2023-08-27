using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCBattleUI : MonoBehaviour
{
    private Slider _hpSlider;

    public float MinimumValue
    {
        get => _hpSlider.minValue;
        set
        {
            _hpSlider.minValue = value;
        }
    }
    public float MaximumValue
    {
        get => _hpSlider.maxValue;
        set
        {
            _hpSlider.maxValue = value;
        }
    }

    public float Value
    {
        get => _hpSlider.value;
        set
        {
            _hpSlider.value = value;
        }
    }

    private void Awake()
    {
        _hpSlider = gameObject.GetComponent<Slider>();
    }

    private void OnEnable()
    {
        GetComponent<Canvas>().enabled = true;
    }
    private void OnDisable()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
