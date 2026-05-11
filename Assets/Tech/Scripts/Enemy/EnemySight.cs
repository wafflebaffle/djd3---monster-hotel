using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField] private float sightRange;
    [SerializeField] private Transform sightTrans;
    [SerializeField] private float viewAngle = 120;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float timeUntilLoseTarget;
    private float _timer;
    private RoomNotifier _respectiveRoom;
    private bool _canFollowPlayer;
    private Transform _saveTarget;
    public Transform Target { get; private set; }

    private void Start()
    {
        _respectiveRoom = GetComponentInParent<RoomNotifier>();
        _respectiveRoom.OnPlayerEnter += canFollow => _canFollowPlayer = canFollow;
    }

    private void Update()
    {   
        if(_canFollowPlayer)
        {
            _timer += Time.deltaTime;
            Target = SeekTarget();

            if (!Target && timeUntilLoseTarget > _timer)
            {
                Target = _saveTarget;
            }
        }
        else Target = null;
    }

    private Transform SeekTarget()
    {
        // Step 1: Get all potential targets within view radius (Sphere check - quick filter)
        Collider[] potentialTargets = Physics.OverlapSphere(sightTrans.position, sightRange, playerMask);
        
        Transform closestTarget = null;
        float closestDistance = sightRange;
        
        foreach (Collider targetCollider in potentialTargets)
        {
            Transform targetTransform = targetCollider.transform;
            Vector3 directionToTarget = (targetTransform.position - sightTrans.position).normalized;
            float distanceToTarget = Vector3.Distance(sightTrans.position, targetTransform.position);
            
            // Step 2: Check if target is within field of view cone (direction test)
            float angleToTarget = Vector3.Angle(sightTrans.forward, directionToTarget);
            
            if (angleToTarget < viewAngle / 2)
            {
                // Target is visible and within FOV!
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = targetTransform;
                    if (!_saveTarget) _saveTarget = closestTarget;
                }
            }
        }

        if(closestTarget == null) Debug.Log("Mata-te");
        return closestTarget;
    }

    void OnDrawGizmos()
    {
        if (sightTrans == null) return;
        
        // Draw view radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sightTrans.position, sightRange);
        
        // Draw FOV cone
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(sightTrans.position, leftBoundary * sightRange);
        Gizmos.DrawRay(sightTrans.position, rightBoundary * sightRange);
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }
}
