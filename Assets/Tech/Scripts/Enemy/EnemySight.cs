using Unity.VisualScripting;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField] private float sightRange;
    [SerializeField] private LayerMask walls;
    [SerializeField] private float obstaclesAvoidanceDistance;
    [SerializeField] private float checkTimer = 2.0f;
    private float _timer;
    private RoomNotifier _respectiveRoom;
    private bool _canFollowPlayer;
    public Transform Target { get; private set; }

    private void Start()
    {
        _respectiveRoom = GetComponentInParent<RoomNotifier>();
        _respectiveRoom.OnPlayerEnter += canFollow => _canFollowPlayer = canFollow;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        RaycastHit hit;

        if(_timer >= checkTimer)
        {
        if (Physics.Raycast(transform.position, transform.forward, out hit,sightRange))
        {
            if (hit.collider.TryGetComponent(out PlayerStats player))
            {
                Target = player.transform;
            }
            else
            {
                if (hit.transform.gameObject.layer == walls)
                    Target = hit.transform;
            }
        }
        _timer = 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*sightRange);
    }
}
