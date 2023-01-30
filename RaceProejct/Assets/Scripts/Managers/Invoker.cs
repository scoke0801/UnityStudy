using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Invoker : MonoBehaviour
{
    private bool _isRecording;
    private bool _isReplaying;
    private float _replayTime;
    private float _recordingTime;
    private SortedList<float, Command> _recordedCommands = new SortedList<float, Command>();

    // 호출자에게 기록과 리플레이의 책임을 부여한다는 점을 유의.
    // 커멘드를 기록 및 저장하고 리플레이하는 책임을 별도의 클래스로 분리하는 게 좋다.

    // 타임 스탬프 정밀도 문제?

    public void ExecuteCommand(Command command)
    {
        command.Execute();

        if (_isRecording)
        {
            _recordedCommands.Add(_recordingTime, command);
        }

        DebugWrapper.Log("Recorded Time :" + _recordingTime);
        DebugWrapper.Log("Recorded Command: " + command);
    }

    public void Record()
    {
        _recordingTime = 0.0f;
        _isReplaying = true;
    }

    public void Replay()
    {
        _replayTime = 0.0f;
        _isReplaying = true;

        if(_recordedCommands.Count <= 0)
        {
            DebugWrapper.LogError("No commands to replay!");   
        }

        _recordedCommands.Reverse();
    }

    private void FixedUpdate()
    {
        if (_isRecording)
        {
            _recordingTime += Time.fixedDeltaTime;
        }

        if (_isReplaying)
        {
            _replayTime += Time.deltaTime;

            if (_recordedCommands.Any())
            {
                if(Mathf.Approximately( _replayTime, _recordedCommands.Keys[0]))
                {
                    DebugWrapper.Log("Replay Time : " + _replayTime);
                    DebugWrapper.Log("Replay Command : " + _recordedCommands.Values[0]);

                    _recordedCommands.Values[0].Execute();
                    _recordedCommands.RemoveAt(0);
                }
            }
            else
            {
                _isReplaying = false;
            }
        }
    }
}
