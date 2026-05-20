using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private RoomType type;

    public Transform Entrance => entrance;
    public RoomType Type => type;


    public enum RoomType
    {
        Small,
        Big
    }
}
