using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;


public class QuestReporter : MonoBehaviour
{
    #region Variables
    [SerializeField] private QuestCategory _category;

    [SerializeField] QuestTaskTarget _target;

    [SerializeField] private int _successCount;

    [SerializeField] private string[] _colliderTargets;
    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReportIfPassCondition(collision);
    }
    #endregion

    #region Public Methods
    public void Report()
    {
        QuestSystem.Instance.ReceiveReport(_category, _target, _successCount);
    }
    #endregion

    #region Private Methods
    private void ReportIfPassCondition(Component other)
    {
        if(_colliderTargets.Any(x=>other.CompareTag(x)))
        {
            Report();
        }
    }
    #endregion
}