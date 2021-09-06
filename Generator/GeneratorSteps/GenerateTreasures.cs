using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class GenerateTreasures : IGeneratorStep {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private List<RoomValueTreasure> RoomValues;
    private Room CurrentRoom;

    private float HighestAverageDistanceValue = 0;


    public GenerateTreasures(LevelLayoutData levelLayoutData, LevelRequirements levelRequirements) {
        LevelLayoutData = levelLayoutData;
        LevelRequirements = levelRequirements;
    }

    public IEnumerator Start(LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        //LevelRequirements = levelRequirements;
        //LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        yield return LevelLayoutGenerator.StartCoroutine(CalculateAndSortRoomValue());
        yield return LevelLayoutGenerator.StartCoroutine(DetermineTreasureCells());

        yield return LevelLayoutGenerator.Yield();
        callback();
    }

    private IEnumerator CalculateAndSortRoomValue() {
        RoomValues = new List<RoomValueTreasure>();

        var roomList = new List<Room>(LevelLayoutData.Rooms);
        roomList.Remove(LevelLayoutData.StartingRoom); // Starting Room has no treasures.
        roomList.Remove(LevelLayoutData.BossRoom); // Boss Room has no treasures.
        roomList.Sort((data1, data2) => data1.Rect.position.x.CompareTo(data2.Rect.position.x));

        foreach (var room in roomList) {
            CurrentRoom = room;
            var roomValue = new RoomValueTreasure(room);
            RoomValues.Add(roomValue);

            HighestAverageDistanceValue = Mathf.Max(HighestAverageDistanceValue, roomValue.AverageDistanceValue);

            yield return LevelLayoutGenerator.Yield();
        }

        RoomValues.Sort((roomValue1, roomValue2) => roomValue1.TotalDistanceValue.CompareTo(roomValue2.TotalDistanceValue));
        RoomValues.Reverse();
        CurrentRoom = null;

        yield return LevelLayoutGenerator.Yield();
    }

    private IEnumerator DetermineTreasureCells() {
        float minimumDistancePercent = 0.1f;
        float baseSpawnChance = 0.2f;
        int minimumTreasures = 7;

        int treausreCount = 0;
        foreach (var roomValue in RoomValues) {
            float roomIntensity = roomValue.AverageDistanceValue / HighestAverageDistanceValue;
            int numberOfTreasures = roomValue.AverageDistanceValue >= minimumDistancePercent && RandomM.Chance(roomIntensity + baseSpawnChance) ? 1 : 0;

            roomValue.NumberOfTreasureCells = numberOfTreasures;
            treausreCount += numberOfTreasures;

            for (int i = 0; i < numberOfTreasures; i++) {
                var treasureCell = roomValue.Room.GetCellsByType(CellType.Default).RandomElement();
                treasureCell.CellType = CellType.Treasure;
                yield return LevelLayoutGenerator.Yield();
            }

            yield return LevelLayoutGenerator.Yield();
        }

        if (treausreCount < minimumTreasures) {
            int treasuresToAdd = minimumTreasures - treausreCount;
            for (int i = 0; i < treasuresToAdd; i++) {
                var treasureCell = LevelLayoutData.GetCellsByType(CellType.Default).RandomElement();
                treasureCell.CellType = CellType.Treasure;
            }
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

public class RoomValueTreasure : IComparable<RoomValueTreasure> {
    public Room Room;
    public int CellsCount;
    public float TotalDistanceValue;
    public float AverageDistanceValue;
    public int NumberOfTreasureCells;


    public RoomValueTreasure(Room room) {
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

    public int CompareTo(RoomValueTreasure other) {
        int distance = TotalDistanceValue.CompareTo(other.TotalDistanceValue);
        if (distance != 0) {
            return distance;
        }
        return CellsCount.CompareTo(other.CellsCount);
    }

    #region Debug
    public string WriteDebug(LevelRequirements levelRequirements) {
        return "{0:N2}= Treasures:{1}".FormatWith(TotalDistanceValue, NumberOfTreasureCells);
    }
    #endregion
}
