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
    public Transform Target => _saveTarget;

    private void OnEnable()
    {
        if (!_respectiveRoom) _respectiveRoom = GetComponentInParent<RoomNotifier>();
        _respectiveRoom.OnPlayerEnter += canFollow => _canFollowPlayer = canFollow;
    }

    private void OnDisable()
    {
        _respectiveRoom.OnPlayerEnter -= canFollow => _canFollowPlayer = canFollow;
    }

    public bool GetTarget(Transform submitTarget = null)
    {   
        Transform target = submitTarget;

        if(_canFollowPlayer)
        {
            if(target != null)

            _timer += Time.deltaTime;
            target = SeekTarget();

            if (target != null)
            {
                _saveTarget = target;
                _timer = 0;
            }
            else if (_saveTarget != null && _timer <= timeUntilLoseTarget)
            {
                target = _saveTarget;
            }
            else
            {
                target = null;
                _saveTarget = null;
            }
        }
        else
        {
            target = null;
            _timer = 0;
            _saveTarget = null;
        }

        Debug.Log("Is player on room: " + _canFollowPlayer + " and the target is: " + target);
        return target;
    }

    public void SetTarget(Transform target)
    {
        _saveTarget = target;
    }

    private Transform SeekTarget()
    {
        Collider[] potentialTargets = Physics.OverlapSphere(sightTrans.position, sightRange, playerMask);
        
        Transform closestTarget = null;
        float closestDistance = sightRange;
        
        foreach (Collider targetCollider in potentialTargets)
        {
            Transform targetTransform = targetCollider.transform;
            Vector3 directionToTarget = (targetTransform.position - sightTrans.position).normalized;
            float distanceToTarget = Vector3.Distance(sightTrans.position, targetTransform.position);
            
            float angleToTarget = Vector3.Angle(sightTrans.forward, directionToTarget);
            
            if (angleToTarget < viewAngle / 2)
            {
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = targetTransform;
                    if (!_saveTarget) _saveTarget = closestTarget;
                }
            }
        }

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
}
