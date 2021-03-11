using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

public class AddBossRoom : IGeneratorStep {
    private LevelRequirements LevelRequirements;
    private LevelLayoutData LevelLayoutData;
    private LevelLayoutGenerator LevelLayoutGenerator;

    private Rect StartingZone = new Rect();//new Rect(-3, -3, 6, 6);
    private int MaxShimmies = 10000;
    private Room BossRoom;

    public IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, System.Action callback) {
        LevelRequirements = levelRequirements;
        LevelLayoutData = levelLayoutData;
        LevelLayoutGenerator = levelLayoutGenerator;

        var size = Vector2.one * Random.Range(10, 16);
        BossRoom = GenerateRoom(size);

        int maxShimmies = MaxShimmies;
        LastShimmyDirection = Vector2.zero;
        while (IsRoomColliding(BossRoom) && maxShimmies-- > 0) {
            ShimmyRoom(BossRoom);
            yield return LevelLayoutGenerator.Yield();
        }


        if (maxShimmies <= 0) {
            // Failed to place, force it.
            maxShimmies = MaxShimmies;
            while (IsRoomColliding(BossRoom) && maxShimmies-- > 0) {
                BossRoom.Rect.position += LastShimmyDirection;
                yield return LevelLayoutGenerator.Yield();
            }
        }

        // Might create a bug, but good enough, and would be almost impossibly rare to happen.
        if (maxShimmies > 0) {
            LevelLayoutData.Rooms.Add(BossRoom);
            LevelLayoutData.BossRoom = BossRoom;
            //levelLayoutData.EndingCell = LevelLayoutData.BossRoom.GetCellsByType(CellType.Default).RandomElement();
        }

        yield return LevelLayoutGenerator.Yield();

        callback();
    }

    #region Room Generation
    private Room GenerateRoom(Vector2 size) {
        var position = new Vector2(Random.Range((int)StartingZone.xMin, (int)StartingZone.xMax + 1), Random.Range((int)StartingZone.yMin, (int)StartingZone.yMax + 1));
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
            Vector2.up, new List<Vector2> { Vector2.left, Vector2.up, Vector2.up, Vector2.up, Vector2.right }
        }, {
            Vector2.right, new List<Vector2> { Vector2.up, Vector2.right, Vector2.right, Vector2.right, Vector2.down }
        }, {
            Vector2.down, new List<Vector2> { Vector2.right, Vector2.down, Vector2.down, Vector2.down, Vector2.left }
        }, {
            Vector2.left, new List<Vector2> { Vector2.down, Vector2.left, Vector2.left, Vector2.left, Vector2.up }
        }
    };
    #endregion

    public void DrawDebug() {
        if (BossRoom != null) {
            GizmosM.DrawRect(BossRoom.Rect, Color.green);
        }
    }

    public string WriteDebug() {
        return "";
    }
}
