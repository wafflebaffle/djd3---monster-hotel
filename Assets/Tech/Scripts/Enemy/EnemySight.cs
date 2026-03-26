using UnityEngine;

public class EnemySight : MonoBehaviour
{
    private RoomNotifier _respectiveRoom;
    private bool _canFollowPlayer;

    private void Start()
    {
        _respectiveRoom = GetComponentInParent<RoomNotifier>();
        _respectiveRoom.OnPlayerEnter += FollowPlayer => _canFollowPlayer = FollowPlayer;  
    }

    private void Update()
    {
        
    }
}
