using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform entrance;

    public Transform Entrance => entrance;
}
