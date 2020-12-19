using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid {
    public Cell[,] Grid;

    public CellGrid(int width, int height) {
        Grid = new Cell[width, height];
    }

    public List<Cell> GetNeighborsCardinal(Point cellPoint) {
        List<Cell> cells = new List<Cell>();
        foreach (var direction in CellCardinalNeighborPoints) {
            Cell cell = GetCell(cellPoint + direction);
            if (cell != null) {
                cells.Add(cell);
            }
        }
        return cells;
    }

    public bool IsValidCell(Point cellPoint) {
        return cellPoint.X >= Grid.GetLowerBound(0) && cellPoint.X < Grid.GetUpperBound(0)
            && cellPoint.Y >= Grid.GetLowerBound(1) && cellPoint.Y < Grid.GetUpperBound(1);
    }

    public Cell GetCell(Point point) {
        return IsValidCell(point) ? Grid[point.X, point.Y] : null;
    }



    public static List<Point> CellCardinalNeighborPoints = new List<Point> {
        new Point(0, 1),
        new Point(1, 0),
        new Point(0, -1),
        new Point(-1, 0)
    };
}
