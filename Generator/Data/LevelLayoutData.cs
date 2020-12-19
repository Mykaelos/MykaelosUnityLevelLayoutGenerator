using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayoutData {
    public List<Room> Rooms;
    public Dictionary<Vector3, Cell> Cells;
    public CellGrid CellGrid;
    public Rect LevelRect;

    public Cell StartingCell;
    public Cell EndingCell;


    public LevelLayoutData() {
        Rooms = new List<Room>();
        Cells = new Dictionary<Vector3, Cell>();
    }

    public List<Cell> GetCellsByType(CellType cellType) {
        var possiableCells = new List<Cell>();
        foreach (var cell in Cells.Values) {
            if (cell.CellType.Is(cellType)) {
                possiableCells.Add(cell);
            }
        }

        return possiableCells;
    }
}
