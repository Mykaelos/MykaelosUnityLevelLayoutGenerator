using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell { // A single 1*1 cell in the game world.
    public Room ParentRoom;
    public Vector3 Position; // Center of the cell
    public Point GridPoint;

    public CellType CellType {
        get { return _CellType;  }
        set { _CellType = value; UpdateDebugColor(); }
    }
    private CellType _CellType;

    public int DistanceFromStart {
        get { return _DistanceFromStart; }
        set { _DistanceFromStart = value; UpdateDebugColor(); }
    }
    private int _DistanceFromStart;

    private Color DebugColor = DEFAULT_COLOR;


    public Cell(Room parentRoom, Vector3 position, CellType cellType) {
        ParentRoom = parentRoom;
        Position = position;
        CellType = cellType;
        DistanceFromStart = -1;
        UpdateDebugColor();
    }

    public void DrawDebug(Color? color = null) {
        if (color == null) {
            Gizmos.color = DebugColor;
        }
        else {
            Gizmos.color = (Color)color;
        }

        Gizmos.DrawWireCube(new Vector3(Position.x, Position.y, -0.2f), new Vector3(1, 1, 0) * 0.9f);
    }

    private void UpdateDebugColor() {
        switch (CellType) {
            case CellType.Default:
                DebugColor = DEFAULT_COLOR.SetA(DistanceFromStart == -1 ? 0.2f : Mathf.Lerp(0.05f, 1f, DistanceFromStart / 50f));
                break;
            case CellType.Wall:
                DebugColor = Color.gray;
                break;
            case CellType.StartPortal:
                DebugColor = Color.green;
                break;
            case CellType.EndPortal:
                DebugColor = Color.red;
                break;
            case CellType.Treasure:
                DebugColor = "#ff00ff".HexAsColor(); // Purple
                break;
            case CellType.Enemy:
                DebugColor = "#ffa500".HexAsColor(); // Orange
                break;
            default:
                DebugColor = Color.red;
                break;
        }
    }

    private static readonly Color DEFAULT_COLOR = new Color(0, 1, 1, 0.2f);
}

public enum CellType {
    Default,
    Wall,
    StartPortal,
    EndPortal,
    Treasure,
    Enemy
}
