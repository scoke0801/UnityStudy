using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    [Tooltip("Task를 구분하기 위한 이름")]
    [Header("Text")]
    [SerializeField]
    private string _codeName;

    [Tooltip("Task에 대한 설명")]
    [SerializeField]
    private string _description;

    [Tooltip("Task에 대해 처리하는 모듈")]
    [Header("Action")]
    [SerializeField]
    private TaskAction _action;

    [Header("Setting")]
    [SerializeField]
    private int _needSuccessToComplete;

    public int IsSucess{get; private set;}
    public string CodeName => _codeName;
    public string Description => _description;
    public int NeedSuccessToComplete => _needSuccessToComplete;

    public void RecieveReport(int successCount)
    {
        IsSucess = _action.Run(this, IsSucess, successCount);
    }
}