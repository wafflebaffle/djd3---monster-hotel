using UnityEngine;

public class RoomArea : MonoBehaviour
{
    public float Xmax { get; private set; }
    public float Xmin { get; private set; }
    public float Zmax { get; private set; }
    public float Zmin { get; private set; }

    // Configure game area limits object
    private void Awake()
    {
        // Get the sprite renderer and its bounds
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Bounds bounds = mesh.bounds;

        // Determine and keep game area limits
        Xmax = bounds.min.x;
        Xmin = bounds.max.x;
        Zmax = bounds.min.z;
        Zmin = bounds.max.z;
    }

    // Determine random position within game area
    public Vector2 RandomPosition(float margin)
    {
        return new Vector2(
            Random.Range(Xmin * margin, Xmax * margin),
            Random.Range(Zmin * margin, Zmax * margin));
    }

    // Determine opposite position within game area
    public Vector2 OppositePosition(char wall, Vector2 currentPos)
    {
        Vector2 newPosition = wall switch
        {
            'E' => new Vector2(Xmax * 0.9f, currentPos.y),
            'W' => new Vector2(Xmin * 0.9f, currentPos.y),
            'N' => new Vector2(currentPos.x, Zmax * 0.9f),
            'S' => new Vector2(currentPos.x, Zmin * 0.9f),
            _ => new Vector2(),
        };
        return newPosition;
    }

    // Draw gizmos, namely borders around the game area
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blueViolet;

        Vector3[] points = new Vector3[]
        {
            new Vector3(Xmin, 0, Zmin),
            new Vector3(Xmin, 0, Zmax),

            new Vector3(Xmin, 0, Zmax),
            new Vector3(Xmax, 0, Zmax),

            new Vector3(Xmax, 0, Zmax),
            new Vector3(Xmax, 0, Zmin),

            new Vector3(Xmax, 0, Zmax),
            new Vector3(Xmin, 0, Zmin)
        };

        Gizmos.DrawLineList(points);
    }
}
