using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// �� ���� �ֿ� ���� ���.
// �Խ���or������: Publisher�� �̺�Ʈ �������� ������ Ư�� ������ �̺�Ʈ�� �����ڿ��� �Խ��� �� �ִ�.
// �̺�Ʈ ����: �̺�Ʈ ������ �����ڿ� �Խ��� ������ �̺�Ʈ ������ �����ϴ� ������ �Ѵ�.
// ������: �����ڴ� �̺�Ʈ ������ ���� Ư�� �̺�Ʈ�� �����ڷ� �ڽ��� ����Ѵ�.

// �����ϴ� Ư�� �̺�Ʈ ������ ����.
public enum RaceEventType
{
    COUNTDOWN,  // ������̴� ī��Ʈ�ٿ� Ÿ�̸Ӱ� ������ ���� ��߼� �ڿ� �־�� ��.
    START,      // Ÿ�̸Ӱ� 0�� �Ǹ� ������̰� ������ �� ����.
    RESTART,    
    PAUSE,      // ���̽� �� �Ͻ�����.
    STOP,       // ġ������ �浹�� ���ϴ� ���, ���̽� ����
    FINISH,     // �÷��̾ ��¼��� ����ϴ� ����.
    QUIT,       // �÷��̾ ���̽��� ������ ���.
}

public class RaceEventBus : MonoBehaviour
{
    private static readonly IDictionary<RaceEventType, UnityEvent> _events = new Dictionary<RaceEventType, UnityEvent>();

    public static void Subscribe(RaceEventType eventType, UnityAction listner)
    {
        UnityEvent thisEvent;

        if( _events.TryGetValue(eventType, out thisEvent)) { 
            thisEvent.AddListener(listner);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listner);
            _events.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(RaceEventType type, UnityAction listener)
    {
        UnityEvent thisEvent;

        if (_events.TryGetValue(type, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    
    public static void Publish(RaceEventType type)
    {
        UnityEvent thisEvent;

        if(_events.TryGetValue(type, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
