using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class GenerateTreasureAndEnemies : IGeneratorStep {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private List<RoomValue> RoomValues;
    private Room CurrentRoom;


    public IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        LevelRequirements = levelRequirements;
        LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        yield return LevelLayoutGenerator.StartCoroutine(CalculateAndSortRoomValue());
        yield return LevelLayoutGenerator.StartCoroutine(DetermineTreasureCells());
        yield return LevelLayoutGenerator.StartCoroutine(DetermineEnemyCells());

        yield return LevelLayoutGenerator.Yield();
        callback();
    }

    private IEnumerator CalculateAndSortRoomValue() {
        RoomValues = new List<RoomValue>();

        var roomList = new List<Room>(LevelLayoutData.Rooms);
        roomList.Sort((data1, data2) => data1.Rect.position.x.CompareTo(data2.Rect.position.x));

        foreach (var room in roomList) {
            CurrentRoom = room;
            RoomValues.Add(new RoomValue(room));
            yield return LevelLayoutGenerator.Yield();
        }

        RoomValues.Sort((roomValue1, roomValue2) => roomValue1.TotalDistanceValue.CompareTo(roomValue2.TotalDistanceValue));
        RoomValues.Reverse();
        CurrentRoom = null;

        yield return LevelLayoutGenerator.Yield();
    }

    private IEnumerator DetermineTreasureCells() {
        int numberOfTreasures = Mathf.CeilToInt(LevelLayoutData.Cells.Count * LevelRequirements.LevelRatioTreasuresPerCell);

        for (int i = 0; i < numberOfTreasures; i++) {
            var roomValue = FindNextTreasureRoom();
            if (roomValue != null) {
                var treasureCell = roomValue.Room.GetCellsByType(CellType.Default).RandomElement();
                treasureCell.CellType = CellType.Treasure;
                roomValue.NumberOfTreasureCells++;
            }
            else {
                var treasureCell = LevelLayoutData.GetCellsByType(CellType.Default).RandomElement();
                treasureCell.CellType = CellType.Treasure;
                // Find roomValue and add treasurecell count?
            }
            yield return LevelLayoutGenerator.Yield();
        }

        yield return LevelLayoutGenerator.Yield();
    }

    private IEnumerator DetermineEnemyCells() {
        int numberOfEnemies = Mathf.CeilToInt(LevelLayoutData.Cells.Count * LevelRequirements.LevelRatioEnemiesPerCell);

        for (int i = 0; i < numberOfEnemies; i++) {
            var roomValue = FindNextEnemyRoom();
            if (roomValue != null) {
                var enemyCell = roomValue.Room.GetCellsByType(CellType.Default).RandomElement();
                enemyCell.CellType = CellType.Enemy;
                roomValue.NumberOfEnemyCells++;
            }
            else {
                var enemyCell = LevelLayoutData.GetCellsByType(CellType.Default).RandomElement();
                enemyCell.CellType = CellType.Enemy;
            }
            yield return LevelLayoutGenerator.Yield();
        }

        yield return LevelLayoutGenerator.Yield();
    }

    private RoomValue FindNextTreasureRoom() {
        foreach (var roomValue in RoomValues) {
            if (roomValue.NumberOfTreasureCells < (roomValue.MaxTreasures(LevelRequirements))) {
                return roomValue;
            }
        }

        return null;
    }

    private RoomValue FindNextEnemyRoom() {
        foreach (var roomValue in RoomValues) {
            if (roomValue.NumberOfEnemyCells < (roomValue.MaxEnemies(LevelRequirements))) {
                return roomValue;
            }
        }

        return null;
    }

    #region Debug
    public void DrawDebug() {
        if (CurrentRoom != null) {
            GizmosM.DrawRect(CurrentRoom.Rect, Color.green);
        }
    }

    public string WriteDebug() {
        string debugString = "";
        foreach (var roomValue in RoomValues) {
            debugString += roomValue.WriteDebug(LevelRequirements).NL();
        }

        return debugString;
    }
    #endregion
}

public class RoomValue : IComparable<RoomValue> {
    public Room Room;
    public int CellsCount;
    public float TotalDistanceValue;
    public int NumberOfTreasureCells;
    public int NumberOfEnemyCells;


    public RoomValue(Room room) {
        Room = room;
        CellsCount = Room.Cells.Count;
        TotalDistanceValue = CalculateTotalDistanceValue(Room);
    }

    private float CalculateTotalDistanceValue(Room room) {
        float totalDistanceValue = 0;
        foreach (var cell in room.Cells) {
            totalDistanceValue += cell.DistanceFromStart;
        }
        return totalDistanceValue;
    }

    public int MaxTreasures(LevelRequirements levelRequirements) {
        return Mathf.CeilToInt(CellsCount * levelRequirements.RoomRatioMaxTreasuresPerCell);
    }

    public int MaxEnemies(LevelRequirements levelRequirements) {
        return Mathf.CeilToInt(CellsCount * levelRequirements.RoomRatioMaxEnemiesPerCell);
    }

    public int CompareTo(RoomValue other) {
        int distance = TotalDistanceValue.CompareTo(other.TotalDistanceValue);
        if (distance != 0) {
            return distance;
        }
        return CellsCount.CompareTo(other.CellsCount);
    }

    #region Debug
    public string WriteDebug(LevelRequirements levelRequirements) {
        return "{0:N2}: Treasures:{1}/{2}, Enemies:{3}/{4}".FormatWith(TotalDistanceValue, NumberOfTreasureCells, MaxTreasures(levelRequirements), NumberOfEnemyCells, MaxEnemies(levelRequirements));
    }
    #endregion
}
