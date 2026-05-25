using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private RoomType type;

    public Quaternion OriginalQuaternion { get; set; }
    public Transform Entrance => entrance;
    public RoomType Type => type;


    public enum RoomType
    {
        Small,
        Big
    }
}
