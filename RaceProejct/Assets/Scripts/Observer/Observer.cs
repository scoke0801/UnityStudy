using System.Collections;
using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    public abstract void Notify(ObserverSubject subject);
}