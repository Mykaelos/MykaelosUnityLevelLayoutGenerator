using UnityEngine;
using UnityEngine.UI;

public class LevelLayoutGeneratorTestSceneController : MonoBehaviour {
    public Text DebugText;
    public LevelRequirements LevelRequirements = new LevelRequirements(500);
    private LevelLayoutGenerator LevelLayoutGenerator;


    private void Awake() {
        LevelLayoutGenerator = new LevelLayoutGenerator(this, DebugText);
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
        LevelLayoutGenerator.GenerateLevel(LevelRequirements);
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

public class GizmosM {
    public static void DrawRect(Rect rect, Color color) {
        Gizmos.color = color;
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y), new Vector3(rect.size.x, rect.size.y));
    }

    public static void DrawLine(Vector2 vector1, Vector2 vector2, Color color) {
        Gizmos.color = color;
        Gizmos.DrawLine(vector1, vector2);
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
