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
        public bool advanceCoverDecision; // 디테일한 decision을 위한
        public int watiRounds;          // 상태에서 대기 조건에 사용.
        public bool repeatShot;         // 반복 사격
        public float watiInCoverTime;   // 엄폐물 안에서 대기할 시간.
        public float coverTimer;         
        public float patrolTimer;        
        public float shotTimer;         
        public float startShootTimer;
        public float currentShoots;
        public float shotsinRounds;
        public float blindEngageTimer; 
    }

}