using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followResponsiveness;
    [SerializeField] private float roamingAngle, battleAngle;
    private bool _roaming;
    private Vector3 _offset;
    private Vector3 _originalRot;
    private Vector3 _originalPos;

    private void Start()
    {
        _offset = transform.position - target.position;
        _originalRot = transform.rotation.eulerAngles;
        _originalPos = transform.position;
        _roaming = true;
    }

    private void Update()
    {
        if (!_roaming) return;

        Vector3 targetPosition = target.position + _offset;

        transform.position = 
            new Vector3(transform.position.x,
                _originalPos.y,
                targetPosition.z);

        if (transform.position.x - targetPosition.x != 0)
        {
            transform.position = 
                new (Mathf
                    .Lerp(transform.position.x, targetPosition.x, 1 - Mathf
                        .Exp(-followResponsiveness * Time.deltaTime)), 
                    _originalPos.y, 
                    transform.position.z);
        }
    }

    public void ActivateBattleCamera(Transform camPos)
    {
        _roaming = false;

        transform.position = camPos.position;
        transform.rotation = Quaternion.Euler(battleAngle,_originalRot.y,_originalRot.z);
    }

    public void ActivateRoamingCamera()
    {
        _roaming = true;

        transform.position = target.position + _offset;
        transform.rotation = Quaternion.Euler(roamingAngle,_originalRot.y,_originalRot.z);
    }
}
