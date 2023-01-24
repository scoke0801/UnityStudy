using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 세 가지 주요 구성 요소.
// 게시자or발행자: Publisher는 이벤트 버스에서 선언한 특정 종류의 이벤트를 구독자에게 게시할 수 있다.
// 이벤트 버스: 이벤트 버스는 구독자와 게시자 사이의 이벤트 전송을 조정하는 역할을 한다.
// 구독자: 구독자는 이벤트 버스를 통해 특정 이벤트의 구독자로 자신을 등록한다.

// 지원하는 특정 이벤트 종류를 노출.
public enum RaceEventType
{
    COUNTDOWN,  // 오토바이는 카운트다운 타이머가 끝날때 까지 출발선 뒤에 있어야 함.
    START,      // 타이머가 0이 되면 오토바이가 움직일 수 있음.
    RESTART,    
    PAUSE,      // 레이스 중 일시정지.
    STOP,       // 치명적인 충돌을 당하는 경우, 레이스 중지
    FINISH,     // 플레이어가 결승선을 통과하는 시점.
    QUIT,       // 플레이어가 레이스를 나가는 경우.
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
