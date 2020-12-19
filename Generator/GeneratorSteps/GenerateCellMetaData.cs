using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCellMetaData : IGeneratorPart {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private Cell CurrentCell;
    private int MaximumDistance = 0;


    public IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        LevelRequirements = levelRequirements;
        LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        yield return LevelLayoutGenerator.StartCoroutine(DetermineLevelSize());
        yield return LevelLayoutGenerator.StartCoroutine(CreateCellGrid());
        yield return LevelLayoutGenerator.StartCoroutine(DetermineCellDistance(LevelLayoutData.StartingCell));
        yield return LevelLayoutGenerator.StartCoroutine(DetermineCellDistance(LevelLayoutData.EndingCell));

        yield return LevelLayoutGenerator.Yield();
        callback();
    }

    private IEnumerator DetermineLevelSize() {
        LevelLayoutData.LevelRect = new Rect();

        foreach (var cell in LevelLayoutData.Cells) {
            LevelLayoutData.LevelRect = LevelLayoutData.LevelRect.StretchToInclude(cell.Value.Position);

            yield return LevelLayoutGenerator.Yield();
        }

        Debug.Log("LevelSize: {0}".FormatWith(LevelLayoutData.LevelRect));
    }

    private IEnumerator CreateCellGrid() {
        LevelLayoutData.CellGrid = new CellGrid((int)LevelLayoutData.LevelRect.width + 1, (int)LevelLayoutData.LevelRect.height + 1);

        foreach (var cell in LevelLayoutData.Cells) {
            CurrentCell = cell.Value;

            var cellPosition = cell.Value.Position;
            Point gridPoint = (Vector2)cellPosition - LevelLayoutData.LevelRect.position;

            //Debug.Log("cellPosition: {0}; gridPosition: {1}; point: {2}".FormatWith(cellPosition, gridPosition, point));
            LevelLayoutData.CellGrid.Grid[gridPoint.X, gridPoint.Y] = CurrentCell;
            CurrentCell.GridPoint = gridPoint;

            yield return LevelLayoutGenerator.Yield();
        }
        CurrentCell = null;
    }

    private IEnumerator DetermineCellDistance(Cell initialCell) {
        initialCell.DistanceFromStart = 0;
        Queue<Cell> cellQueue = new Queue<Cell>();
        cellQueue.Enqueue(initialCell);
        MaximumDistance = 0;

        while (cellQueue.Count > 0) {
            CurrentCell = cellQueue.Dequeue();
            int nextDistance = CurrentCell.DistanceFromStart + 1;
            MaximumDistance = Mathf.Max(MaximumDistance, nextDistance);

            var neighborCells = LevelLayoutData.CellGrid.GetNeighborsCardinal(CurrentCell.GridPoint);
            foreach (var cell in neighborCells) {
                // If the cell.DistanceFromStart is -1, then we haven't filled it in yet (defaults to -1).
                // Otherwise, we only want to change the distance from start if it can be lower for some reason.
                if (!cell.CellType.Has(CellType.Wall) &&
                    (cell.DistanceFromStart == -1 || cell.DistanceFromStart > nextDistance)) {
                    cell.DistanceFromStart = nextDistance;
                    cellQueue.Enqueue(cell);
                }
            }
            yield return LevelLayoutGenerator.Yield();
        }

        CurrentCell = null;
        Debug.Log("Maximum Cell Distance from {0}: {1}".FormatWith(initialCell, MaximumDistance));

        yield return LevelLayoutGenerator.Yield();
    }

    #region Debug
    public void DrawDebug() {
        if (CurrentCell != null) {
            CurrentCell.DrawDebug(Color.green);
        }
    }

    public string WriteDebug() {
        return "LevelSize: {0}".FormatWith(LevelLayoutData.LevelRect).NL() +
            "Maximum Distance: {0}".FormatWith(MaximumDistance);
    }
    #endregion
}
