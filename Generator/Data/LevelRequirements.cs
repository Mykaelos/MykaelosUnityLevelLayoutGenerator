using UnityEngine;

[System.Serializable]
public class LevelRequirements {
    [Tooltip("Minimum number of cells generated for the level.")]
    public int CellsMinimumCount;

    [Tooltip("Random Room Size: X/Y is minimum size, W/H is maximum size.")]
    public Rect RoomSizeRange = new Rect(new Vector2(2, 2), new Vector2(8, 8));

    [Tooltip("Ratio of number of treasures per number of cells in the level.")]
    public float LevelRatioTreasuresPerCell = 2f / 100f;

    [Tooltip("Max Ratio of treasures per cells in each room.")]
    public float RoomRatioMaxTreasuresPerCell = 1f / 40f;

    [Tooltip("Ratio of number of enemies per number of cells in the level.")]
    public float LevelRatioEnemiesPerCell = 5f / 100f;

    [Tooltip("Max Ratio of enemies per cells in each room.")]
    public float RoomRatioMaxEnemiesPerCell = 1f / 40f;

    public LevelRequirements(int cellsMinimumCount) {
        CellsMinimumCount = cellsMinimumCount;
    }
}
