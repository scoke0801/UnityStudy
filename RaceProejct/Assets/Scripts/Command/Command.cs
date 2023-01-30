using UnityEditor;
using UnityEngine;

// 호출자 : 명령을 실행하는 방법을 알고 실행한 명령을 즐겨찾기 할 수도 있는 객체다.
// 수신자 : 명령을 받아서 수행할 수 있는 종류의 객체
// CommandBase는 개별 ConcreteCommand 클래스가 무조건 상속해야 하는 추상 클래스.
// 호출자가 특정 명령을 실행하기 위해 호출할 수 있는 Execute메서드를 노출한다.

public abstract class Command
{
    public abstract void Execute();
}

public class ToggleTurbo : Command
{
    private BikeController _controller;

    public ToggleTurbo(BikeController controller)
    {
        _controller = controller;
    }

    public override void Execute()
    {
        _controller.ToggleTurbo();
    }
}

public class TurnLeft : Command
{
    private BikeController _controller;

    public TurnLeft(BikeController controller) { _controller= controller; }

    public override void Execute()
    {
        _controller.Turn(Direction.Left);
    }
}

public class TurnRight : Command
{
    private BikeController _controller;

    public TurnRight(BikeController controller)
    {
        _controller = controller;
    }

    public override void Execute()
    {
        _controller.Turn(Direction.Right);
    }
}
