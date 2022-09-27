using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskDescriptor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Color _normalColor;

    [SerializeField]
    private Color _taskCompleteColor;

    [SerializeField]
    private Color _taskSuccessColor;

    [SerializeField]
    private Color _strikeColor;

    public void UpdateText(string text)
    {
        this._text.fontStyle = FontStyles.Normal;
        this._text.text = text;
    }

    public void UpdateText(Task task)
    {
        _text.fontStyle = FontStyles.Normal;

        if(task.IsComplete)
        {
            var colorCode = ColorUtility.ToHtmlStringRGB(_taskCompleteColor);
            _text.text = BuildText(task, colorCode, colorCode);
        }
        else
        {
            _text.text = BuildText(task, ColorUtility.ToHtmlStringRGB(_normalColor), ColorUtility.ToHtmlStringRGB(_taskSuccessColor));
        }
    }

    public void UpdateTextUsingStrikeThrough(Task task)
    {
        var colorCode = ColorUtility.ToHtmlStringRGB(_strikeColor);
        _text.fontStyle = FontStyles.Strikethrough;
        _text.text = BuildText(task, colorCode, colorCode);
    }
    private string BuildText(Task task, string textColorCode, string successCountColorCode)
    {
        return $"<color=#{textColorCode}>¡Ü {task.Description} <color=#{successCountColorCode}>{task.CurrentSuccess}</color>/{task.NeedSuccessToComplete}</color>";
    }
}
