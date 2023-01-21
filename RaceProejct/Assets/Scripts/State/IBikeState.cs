using UnityEditor;
using UnityEngine;

public enum Direction
{
    Left = -1,
    Right = 1,
}


public interface IBikeState
{
    void Handle(BikeController controller);
}

