using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // So we can see it in the editor.
public class Room {
    public Rect Rect;
    public List<Cell> Cells = new List<Cell>();
    public List<Cell> WallCells = new List<Cell>();


    public Room(Rect rect) {
        Rect = rect;
    }

    public int GetSize() {
        return (int)Rect.width * (int)Rect.height;
    }

    public Cell GetCell(Vector3 Position) {
        foreach (var cell in Cells) {
            if (cell.Position.Equals(Position)) {
                return cell;
            }
        }
        return null;
    }

    public List<Cell> GetCellsByType(CellType cellType) {
        var possiableCells = new List<Cell>();
        foreach (var cell in Cells) {
            if (cell.CellType.Is(cellType)) {
                possiableCells.Add(cell);
            }
        }

        return possiableCells;
    }

    public void DrawDebug() {
        GizmosM.DrawRect(new Rect(Rect.position - (Vector2.one * 0.5f), Rect.size), ROOM_COLOR); //TODO - Fix this for real in the generator.

        foreach (var cell in Cells) {
            cell.DrawDebug();
        }

        foreach (var wallCells in WallCells) {
            wallCells.DrawDebug();
        }
    }

    private static Color ROOM_COLOR = Color.yellow.SetA(1);
}
