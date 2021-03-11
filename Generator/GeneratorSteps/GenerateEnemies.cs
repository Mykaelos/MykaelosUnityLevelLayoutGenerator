using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class GenerateEnemies : IGeneratorStep {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private List<RoomValue2> RoomValues;
    private Room CurrentRoom;

    private float HighestAverageDistanceValue = 0;


    public IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        LevelRequirements = levelRequirements;
        LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        yield return LevelLayoutGenerator.StartCoroutine(CalculateAndSortRoomValue());
        yield return LevelLayoutGenerator.StartCoroutine(DetermineEnemyCells());

        yield return LevelLayoutGenerator.Yield();
        callback();
    }

    private IEnumerator CalculateAndSortRoomValue() {
        RoomValues = new List<RoomValue2>();

        var roomList = new List<Room>(LevelLayoutData.Rooms);
        roomList.Remove(LevelLayoutData.StartingRoom); // Starting Room has no enemies.
        roomList.Sort((data1, data2) => data1.Rect.position.x.CompareTo(data2.Rect.position.x));

        foreach (var room in roomList) {
            CurrentRoom = room;
            var roomValue = new RoomValue2(room);
            RoomValues.Add(roomValue);

            HighestAverageDistanceValue = Mathf.Max(HighestAverageDistanceValue, roomValue.AverageDistanceValue);

            yield return LevelLayoutGenerator.Yield();
        }

        RoomValues.Sort((roomValue1, roomValue2) => roomValue1.TotalDistanceValue.CompareTo(roomValue2.TotalDistanceValue));
        RoomValues.Reverse();
        CurrentRoom = null;

        yield return LevelLayoutGenerator.Yield();
    }

    private IEnumerator DetermineEnemyCells() {
        // HighestAverageDistanceValue determines the most intense room. Scale down from there.
        float enemyDensityMax = 0.2f;

        foreach (var roomValue in RoomValues) {
            float roomIntensity = roomValue.AverageDistanceValue / HighestAverageDistanceValue;
            float roomDensity = roomIntensity * enemyDensityMax;
            int numberOfEnemies = (int)(roomDensity * (float)roomValue.CellsCount);
            roomValue.NumberOfEnemyCells = numberOfEnemies;

            for (int i = 0; i < numberOfEnemies; i++) {
                var enemyCell = roomValue.Room.GetCellsByType(CellType.Default).RandomElement();
                enemyCell.CellType = CellType.Enemy;
                yield return LevelLayoutGenerator.Yield();
            }

            yield return LevelLayoutGenerator.Yield();
        }

        yield return LevelLayoutGenerator.Yield();
    }

    #region Debug
    public void DrawDebug() {
        if (CurrentRoom != null) {
            GizmosM.DrawRect(CurrentRoom.Rect, Color.green);
        }
    }

    public string WriteDebug() {
        string debugString = "HighestAverageDistanceValue : {0:N2}".FormatWith(HighestAverageDistanceValue).NL();
        foreach (var roomValue in RoomValues) {
            debugString += roomValue.WriteDebug(LevelRequirements).NL();
        }

        return debugString;
    }
    #endregion
}

public class RoomValue2 : IComparable<RoomValue2> {
    public Room Room;
    public int CellsCount;
    public float TotalDistanceValue;
    public float AverageDistanceValue;
    public int NumberOfEnemyCells;


    public RoomValue2(Room room) {
        Room = room;
        CellsCount = Room.Cells.Count;
        TotalDistanceValue = CalculateTotalDistanceValue(Room);
        AverageDistanceValue = TotalDistanceValue / (float)CellsCount;
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

    public int CompareTo(RoomValue2 other) {
        int distance = TotalDistanceValue.CompareTo(other.TotalDistanceValue);
        if (distance != 0) {
            return distance;
        }
        return CellsCount.CompareTo(other.CellsCount);
    }

    #region Debug
    public string WriteDebug(LevelRequirements levelRequirements) {
        return "{0:N2}= Enemies:{1}".FormatWith(TotalDistanceValue, NumberOfEnemyCells);
    }
    #endregion
}
