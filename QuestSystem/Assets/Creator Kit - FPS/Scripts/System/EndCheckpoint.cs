﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCheckpoint : MonoBehaviour
{
    [SerializeField]
    private QuestReporter questReporter;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Controller>() == null)
            return;

        questReporter.Report();

        QuestSystem.Instance.CompleteWaitngQuests();
        QuestSystem.Instance.Save();

        GameSystem.Instance.StopTimer();
        GameSystem.Instance.FinishRun();
        Destroy(gameObject);
    }
}
