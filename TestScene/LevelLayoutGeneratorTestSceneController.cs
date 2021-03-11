using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Generator;
using UnityEngine;
using UnityEngine.UI;

public class LevelLayoutGeneratorTestSceneController : MonoBehaviour {
    public LevelRequirements LevelRequirements = new LevelRequirements(500);

    private Text DebugText;
    private LevelLayoutGenerator LevelLayoutGenerator;
    private List<IGeneratorStep> GeneratorSteps = new List<IGeneratorStep>();


    private void Awake() {
        LevelLayoutGenerator = new LevelLayoutGenerator(this, DebugText);

        GeneratorSteps = new List<IGeneratorStep> {
            new ClusteredRoomPlacer(),
            new PrepareRoomCells(),
            new PortalPlacer(),
            new SurroundRoomsWithWalls(),
            new GenerateCellMetaData(),
            new GenerateTreasureAndEnemies()
        };

        // Unity breaks prefab links when the library is added to new projects. We can avoid this problem by setting them via code.
        DebugText = this.GetComponentInChild<Text>("DebugText");
        this.GetComponentInChild<Button>("GenerateButton").onClick.AddListener(GenerateLevel);
        this.GetComponentInChild<Button>("IncreaseSpeedButton").onClick.AddListener(IncreaseSpeed);
        this.GetComponentInChild<Button>("DecreaseSpeedButton").onClick.AddListener(DecreaseSpeed);
        this.GetComponentInChild<Button>("ToggleYieldButton").onClick.AddListener(ToggleYield);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateLevel();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            IncreaseSpeed();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            DecreaseSpeed();
        }

        LevelLayoutGenerator.DrawDebugText();
    }

    #region GUI Public Interface Methods
    public void GenerateLevel() {
        LevelLayoutGenerator.GenerateLevel(LevelRequirements, GeneratorSteps);
    }

    public void IncreaseSpeed() {
        LevelLayoutGenerator.IncreaseSpeed();
    }

    public void DecreaseSpeed() {
        LevelLayoutGenerator.DecreaseSpeed();
    }

    public void ToggleYield() {
        LevelLayoutGenerator.ToggleYield();
    }
    #endregion

    private void OnDrawGizmos() {
        if (LevelLayoutGenerator != null) {
            LevelLayoutGenerator.DrawDebug();
        }
    }
}


// TODO
// - Verify that all of the rooms are connected
// - - Grab first cell of each room, and A* to every other room
// - - If not connected, create hallway to nearest neighbor room
// - Pick starting point
// - - Order rooms from center, pick one of the further ones
// - Pick ending point
// - - Order rooms from Starting point, pick one of the further ones
// - Randomly connect rooms
// - - Determine cells/walls that can become doorways
// - - Mark walls as doorway and cells as connected
// - Determine Main path
// - - A* pathfind from start to end, via open doorways
// - For each cell in each room, find distance from main path
// - - A* or floodfill from each cell until something connects with a path cell/room
