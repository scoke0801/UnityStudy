using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    public CustomGridConfig config;

    public Vector3[] horLines;
    public Vector3[] verLines;

    public bool reposition;

    public void OnValidate()
    {
        reposition = true;
    }

    public void RefreshPoints()
    {
        int verLineCnt = config.CellCount.x * 2 + 2;
        int horLineCnt = config.CellCount.y * 2 + 2;

        horLines = new Vector3[horLineCnt];
        verLines = new Vector3[verLineCnt];

        for (int i = 0; i <= config.CellCount.x; ++i)
        {
            verLines[i * 2] = new Vector3(i * config.CellSize.x, config.CellSize.y * config.CellCount.y, 0);
            verLines[i * 2 + 1] = new Vector3(i * config.CellSize.x, 0, 0);
        }

        for (int i = 0; i <= config.CellCount.y; ++i)
        {
            horLines[i * 2] = new Vector3(0, i * config.CellSize.y, 0);
            horLines[i * 2 + 1] = new Vector3(config.CellSize.x * config.CellCount.x, i * config.CellSize.y, 0);
        }
    }
}
