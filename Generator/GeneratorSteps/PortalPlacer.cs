using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class PortalPlacer : IGeneratorStep {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private List<RoomDistanceData> RoomDistanceData;
    private RoomDistanceData ClosestRoomDistance;
    private readonly float IdealDistanceModifier = 0.75f;


    public IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        LevelRequirements = levelRequirements;
        LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        yield return LevelLayoutGenerator.StartCoroutine(GenerateRoomDistanceData());

        yield return LevelLayoutGenerator.StartCoroutine(DeterminePortalRooms());

        yield return LevelLayoutGenerator.StartCoroutine(DeterminePortalCells());

        yield return LevelLayoutGenerator.Yield();

        callback();
    }

    private IEnumerator GenerateRoomDistanceData() {
        RoomDistanceData = new List<RoomDistanceData>();

        for (int i = 0; i < LevelLayoutData.Rooms.Count; i++) {
            var currentRoom = LevelLayoutData.Rooms[i];

            for (int j = i + 1; j < LevelLayoutData.Rooms.Count; j++) {
                var compareRoom = LevelLayoutData.Rooms[j];

                RoomDistanceData.Add(new RoomDistanceData(currentRoom, compareRoom));
                yield return LevelLayoutGenerator.Yield();
            }
        }
    }

    private IEnumerator DeterminePortalRooms() {
        RoomDistanceData.Sort((data1, data2) => data1.Distance.CompareTo(data2.Distance));

        var shortestDistance = RoomDistanceData[0].Distance;
        var longestDistance = RoomDistanceData[RoomDistanceData.Count - 1].Distance;
        var idealDistance = Math.Abs(longestDistance - shortestDistance) * IdealDistanceModifier + shortestDistance;
        Debug.Log("shortestDistance: {0:N2}, longestDistance: {1:N2}, idealDistance: {2:N2}".FormatWith(shortestDistance, longestDistance, idealDistance));

        ClosestRoomDistance = RoomDistanceData[0];
        foreach(var roomDistanceData in RoomDistanceData) {
            if (Math.Abs(roomDistanceData.Distance - idealDistance) < Math.Abs(ClosestRoomDistance.Distance - idealDistance)) {
                ClosestRoomDistance = roomDistanceData;

                yield return LevelLayoutGenerator.Yield();
            }
        }

        Debug.Log("closestRoom: {0}".FormatWith(ClosestRoomDistance.Distance));
    }

    private IEnumerator DeterminePortalCells() {
        var startingCell = ClosestRoomDistance.Room1.GetCellsByType(CellType.Default).RandomElement();
        startingCell.CellType = CellType.StartPortal;
        LevelLayoutData.StartingCell = startingCell;

        var endingCell = ClosestRoomDistance.Room2.GetCellsByType(CellType.Default).RandomElement();
        endingCell.CellType = CellType.EndPortal;
        LevelLayoutData.EndingCell = endingCell;

        yield return LevelLayoutGenerator.Yield();
    }

    public void DrawDebug() {
        foreach (var roomDistanceData in RoomDistanceData) {
            roomDistanceData.DrawDebug(null);
        }

        if (ClosestRoomDistance != null) {
            ClosestRoomDistance.DrawDebug(Color.green);
        }
    }

    public string WriteDebug() {
        string debugString = "";
        foreach (var roomDistanceData in RoomDistanceData) {
            debugString += roomDistanceData.WriteDebug().NL();
        }

        return debugString;
    }
}

public class RoomDistanceData {
    public Room Room1;
    public Room Room2;
    public float Distance;


    public RoomDistanceData(Room room1, Room room2) {
        Room1 = room1;
        Room2 = room2;
        Distance = Vector2.Distance(Room1.Rect.center, Room2.Rect.center);
    }

    public void DrawDebug(Color? color) {
        GizmosM.DrawLine(Room1.Rect.center, Room2.Rect.center, color ?? Color.yellow.SetA(0.5f));
    }

    public string WriteDebug() {
        return "{0:N2}".FormatWith(Distance);
    }
}
