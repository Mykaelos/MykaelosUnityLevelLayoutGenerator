using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class ClusteredRoomPlacer : IGeneratorStep {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private Rect StartingZone = new Rect();//new Rect(-3, -3, 6, 6);
    private int MaxShimmies = 100;
    private Room CurrentRoom;
    private int CurrentCellCount;


    public ClusteredRoomPlacer(LevelLayoutData levelLayoutData, LevelRequirements levelRequirements) {
        LevelLayoutData = levelLayoutData;
        LevelRequirements = levelRequirements;
    }

    public IEnumerator Start(LevelLayoutGenerator levelLayoutGenerator, System.Action callback) {
        //LevelRequirements = levelRequirements;
        //LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        CurrentCellCount = 0;
        while (CurrentCellCount < LevelRequirements.CellsMinimumCount) {
            CurrentRoom = GenerateRoom();
            yield return LevelLayoutGenerator.Yield();

            int maxShimmies = MaxShimmies;
            LastShimmyDirection = Vector2.zero;
            while (IsRoomColliding(CurrentRoom) && maxShimmies-- > 0) {
                ShimmyRoom(CurrentRoom);
                yield return LevelLayoutGenerator.Yield();
            }

            if (maxShimmies > 0) {
                LevelLayoutData.Rooms.Add(CurrentRoom);
                CurrentCellCount += CurrentRoom.GetSize();
            }

            CurrentRoom = null;

            yield return LevelLayoutGenerator.Yield();
        }

        callback();
    }

    #region Room Generation
    private Room GenerateRoom() {
        var position = new Vector2(Random.Range((int)StartingZone.xMin, (int)StartingZone.xMax + 1), Random.Range((int)StartingZone.yMin, (int)StartingZone.yMax + 1));
        var size = new Vector2(Random.Range((int)LevelRequirements.RoomSizeRange.x, (int)LevelRequirements.RoomSizeRange.width + 1), Random.Range((int)LevelRequirements.RoomSizeRange.y, (int)LevelRequirements.RoomSizeRange.height + 1));
        var roomRect = new Rect(position, size);

        return new Room(roomRect);
    }

    private bool IsRoomColliding(Room room) {
        foreach (var testRoom in LevelLayoutData.Rooms) {
            if (testRoom.Rect.Overlaps(room.Rect)) {
                return true;
            }
        }

        return false;
    }

    private void ShimmyRoom(Room room) {
        LastShimmyDirection = ShimmyDirectionOptions.Get(LastShimmyDirection).RandomElement();

        room.Rect.position += LastShimmyDirection;
    }

    private Vector2 LastShimmyDirection = Vector2.zero;
    private static readonly Dictionary<Vector2, List<Vector2>> ShimmyDirectionOptions = new Dictionary<Vector2, List<Vector2>> {
        {
            Vector2.zero, new List<Vector2> { Vector2.up, Vector2.right, Vector2.down, Vector2.left }
        }, {
            Vector2.up, new List<Vector2> { Vector2.left, Vector2.up, Vector2.up, Vector2.right }
        }, {
            Vector2.right, new List<Vector2> { Vector2.up, Vector2.right, Vector2.right, Vector2.down }
        }, {
            Vector2.down, new List<Vector2> { Vector2.right, Vector2.down, Vector2.down, Vector2.left }
        }, {
            Vector2.left, new List<Vector2> { Vector2.down, Vector2.left, Vector2.left, Vector2.up }
        }
    };
    #endregion

    public void DrawDebug() {
        if (CurrentRoom != null) {
            GizmosM.DrawRect(CurrentRoom.Rect, Color.green);
        }
    }

    public string WriteDebug() {
        return "Cells {0} / {1}".FormatWith(CurrentCellCount, LevelRequirements.CellsMinimumCount);
    }
}
