using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Threading;
using Unity.VisualScripting;

public class EnemySight : MonoBehaviour
{
    [SerializeField] private Vector3 sightRange;
    [SerializeField] private Transform sightTrans;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float timeUntilLoseTarget;
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
        Collider[] colliders = new Collider[1];

        if(_canFollowPlayer)
        {
            if (Physics.OverlapBoxNonAlloc(sightTrans.position, sightRange, colliders, Quaternion.identity,playerMask) > 0)
            {
                Target = colliders[0].transform;
                _timer = 0;
            }
            else
            {
                _timer += Time.deltaTime;

                if(_timer >= timeUntilLoseTarget)
                    Target = null;
            }
        }
        else Target = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(sightTrans.position, sightRange);
    }

    private void SetTarget(Transform target)
    {
        Target = target;
    }
}
