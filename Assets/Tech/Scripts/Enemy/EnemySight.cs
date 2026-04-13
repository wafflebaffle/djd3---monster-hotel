using Unity.VisualScripting;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField] private float sightRange;
    [SerializeField] private LayerMask walls;
    [SerializeField] private float obstaclesAvoidanceDistance;
    private RoomNotifier _respectiveRoom;
    private bool _canFollowPlayer;
    public Transform Target { get; private set; }

    private void Start()
    {
        _respectiveRoom = GetComponentInParent<RoomNotifier>();
        _respectiveRoom.OnPlayerEnter += FollowPlayer => _canFollowPlayer = FollowPlayer;  
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit,sightRange))
        {
            if (hit.collider.TryGetComponent(out PlayerStats player))
            {
                Target = player.transform;
            }
            else
            {
                Target = null;
            }
        }
    }
}
