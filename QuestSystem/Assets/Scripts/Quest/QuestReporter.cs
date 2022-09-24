using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestReporter : MonoBehaviour
{
    [SerializeField]
    private Category _category;

    [SerializeField]
    private TaskTarget _target;

    [SerializeField]
    private int _succesCount;

    [SerializeField]
    private string[] _colliderTags;


    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReportIfPassCondition(collision);
    }

    public void Report()
    {
        QuestSystem.Instance.ReceiveReport(_category, _target, _succesCount);
    }

    private void ReportIfPassCondition(Component otehr)
    {
        if( _colliderTags.Any(x => otehr.CompareTag(x)))
        { 
            Report();
        }
    }
}
