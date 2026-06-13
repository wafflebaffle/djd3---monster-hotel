using UnityEngine;

/// <summary>
/// Represents a room in the game environment, including its entrance and type.
/// </summary>
public class Room : MonoBehaviour
{
    /// <summary>
    /// Represents the entrance transform for the object.
    /// </summary>
    [SerializeField] private Transform entrance;
    /// <summary>
    /// Defines the room type
    /// </summary>
    [SerializeField] private RoomType type;

    /// <summary>
    /// Gets or sets the original orientation as a quaternion.
    /// </summary>
    public Quaternion OriginalQuaternion { get; set; }
    /// <summary>
    /// Gets the entrance transform.
    /// </summary>
    public Transform Entrance => entrance;
    /// <summary>
    /// Gets the room type.
    /// </summary>
    public RoomType Type => type;

}
