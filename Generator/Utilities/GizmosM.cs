using UnityEngine;

namespace MykaelosUnityLevelLayoutGenerator.Utilities {
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
}
