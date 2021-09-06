using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using UnityEngine;

public class PrepareRoomCells : IGeneratorStep {
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;


    public PrepareRoomCells(LevelLayoutData levelLayoutData) {
        LevelLayoutData = levelLayoutData;
    }

    public IEnumerator Start(LevelLayoutGenerator levelLayoutGenerator, Action callback) {
        LevelLayoutGenerator = levelLayoutGenerator;

        var roomList = new List<Room>(LevelLayoutData.Rooms);
        roomList.Sort((data1, data2) => data1.Rect.position.x.CompareTo(data2.Rect.position.x));

        foreach (var room in roomList) {
            yield return LevelLayoutGenerator.StartCoroutine(BuildCells(room));
        }

        callback();
    }

    public IEnumerator BuildCells(Room room) {
        room.Cells.Clear();
        Vector3 cellBottomCenter = new Vector3(0, 0);
        int xMin = (int)room.Rect.x;
        int yMin = (int)room.Rect.y;

        int width = (int)room.Rect.width; // Force units to snap to int
        int height = (int)room.Rect.height;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var position = new Vector3(xMin + x, yMin + y) + cellBottomCenter;
                var cell = new Cell(room, position, CellType.Default);
                room.Cells.Add(cell);
                LevelLayoutData.Cells.Add(position, cell);

                yield return LevelLayoutGenerator.Yield();
            }
        }
    }

    public void DrawDebug() { }

    public string WriteDebug() {
        return "";
    }
}
