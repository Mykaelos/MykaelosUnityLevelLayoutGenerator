using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using UnityEngine;

public class SurroundRoomsWithWalls : IGeneratorStep {
    //private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;
    private Cell CurrentCell;


    public SurroundRoomsWithWalls(LevelLayoutData levelLayoutData) {
        LevelLayoutData = levelLayoutData;
    }

    public IEnumerator Start(LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        //LevelRequirements = levelRequirements;
        //LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        var roomList = new List<Room>(LevelLayoutData.Rooms);
        roomList.Sort((data1, data2) => data1.Rect.position.x.CompareTo(data2.Rect.position.x));

        foreach (var room in roomList) {
            yield return LevelLayoutGenerator.StartCoroutine(SurroundRoomWithWalls(room));
        }

        callback();
    }

    private IEnumerator SurroundRoomWithWalls(Room room) {
        foreach(var cell in room.Cells) {
            CurrentCell = cell;
            yield return LevelLayoutGenerator.Yield();

            foreach (var neighborLocation in RelativeNeighborCellLocation) {
                Vector3 checkLocation = cell.Position + neighborLocation;
                if (!LevelLayoutData.Cells.ContainsKey(checkLocation)) {
                    var wallCell = new Cell(room, checkLocation, CellType.Wall);
                    room.WallCells.Add(wallCell);
                    LevelLayoutData.Cells.Add(checkLocation, wallCell);

                    yield return LevelLayoutGenerator.Yield();
                }
            }
            CurrentCell = null;
        }
    }

    public void DrawDebug() {
        if (CurrentCell != null) {
            CurrentCell.DrawDebug(Color.green);
        }
    }

    public string WriteDebug() {
        return "";
    }

    private static readonly List<Vector3> RelativeNeighborCellLocation = new List<Vector3> {
        Vector3.up,
        Vector3.right,
        Vector3.down,
        Vector3.left, 
        Vector3.up + Vector3.right, // Also check corners.
        Vector3.down + Vector3.right,
        Vector3.down + Vector3.left,
        Vector3.up + Vector3.left
    };
}
