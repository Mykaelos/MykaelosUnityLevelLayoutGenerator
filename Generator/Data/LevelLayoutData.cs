using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class LevelLayoutData {
    public List<Room> Rooms;
    public Dictionary<Vector3, Cell> Cells;
    public CellGrid CellGrid;
    public Rect LevelRect;

    public Cell StartingCell;
    public Cell EndingCell;

    public Room StartingRoom;
    public Room BossRoom;


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

    #region DrawDebug - Debug visuals while generating the level
    public string DrawDebugText() {
        //if (DebugText != null) {
            string debugString = "";

            //debugString +=
            //    Duration.GetDurationTimeStamp().NL() +
            //    "YieldTime: {0:N3}\nSpeed: {1:N1}%".FormatWith(YieldManager.YieldTime, YieldManager.SpeedFraction * 100).NL();

            //if (LevelLayoutData != null) {
                if (!Rooms.IsNullOrEmpty()) {
                    debugString += "Total Rooms: {0}".FormatWith(Rooms.Count).NL();
                }
                if (!Cells.IsNullOrEmpty()) {
                    debugString += "Total Cells: {0}".FormatWith(Cells.Count).NL();
                }
                debugString = debugString.NL();
            //}

            //if (CurrentGeneratorStep != null) {
            //    debugString +=
            //        "Running: {0}".FormatWith(CurrentGeneratorStep.GetType().Name).NL()
            //        + CurrentGeneratorStep.WriteDebug();
            //}

        //    DebugText.text = debugString;
        //}


        return debugString;
    }

    public void DrawDebug() {
        //if (LevelLayoutData != null) {
            foreach (var room in Rooms) {
                room.DrawDebug();
            }

            if (LevelRect != Rect.zero) {
                GizmosM.DrawRect(LevelRect, LEVEL_RECT_GREEN);
            }

            if (BossRoom != null) {
                GizmosM.DrawRect(BossRoom.Rect, Color.red);
                //GizmosM.DrawRect(new Rect(LevelLayoutData.BossRoom.Rect.position - (Vector2.one * 0.5f), LevelLayoutData.BossRoom.Rect.size), BOSS_RECT_ORANGE); //TODO - Fix this for real in the generator.
            }

            if (StartingRoom != null) {
                GizmosM.DrawRect(StartingRoom.Rect, Color.green);
                //GizmosM.DrawRect(new Rect(LevelLayoutData.BossRoom.Rect.position - (Vector2.one * 0.5f), LevelLayoutData.BossRoom.Rect.size), BOSS_RECT_ORANGE); //TODO - Fix this for real in the generator.
            }
        //}

        //if (CurrentGeneratorStep != null) {
        //    CurrentGeneratorStep.DrawDebug();
        //}
    }

    private static readonly Color LEVEL_RECT_GREEN = "adff2f".HexAsColor().SetA(0.5f);
    private static readonly Color BOSS_RECT_ORANGE = "FF7000".HexAsColor().SetA(1f);
    #endregion
}
