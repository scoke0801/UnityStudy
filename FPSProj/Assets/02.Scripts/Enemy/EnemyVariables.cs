using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    [Serializable]
    public class EnemyVariables
    {
        // Shoot decision

        // Cover decision

        // Repeat decision

        // Patrol decision

        // Attack decision


        public bool feelAlert;
        public bool heartAlert;
        public bool advanceCoverDecision; // �������� decision�� ����
        public int watiRounds;          // ���¿��� ��� ���ǿ� ���.
        public bool repeatShot;         // �ݺ� ���
        public float watiInCoverTime;   // ���� �ȿ��� ����� �ð�.
        public float coverTimer;         
        public float patrolTimer;        
        public float shotTimer;         
        public float startShootTimer;
        public float currentShoots;
        public float shotsinRounds;
        public float blindEngageTimer; 
    }

}