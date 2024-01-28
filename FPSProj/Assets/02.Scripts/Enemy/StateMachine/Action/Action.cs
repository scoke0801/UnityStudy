using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController controller);

        public virtual void OnReadyAction(StateController controller)
        {

        }
    }
}