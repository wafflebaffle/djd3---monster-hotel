using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followResponsiveness;
    private Vector3 _offset;

    private void Start()
    {
        _offset = transform.position - target.position;
    }

    private void Update()
    {
        Vector3 targetPosition = target.position + _offset;

        transform.position = 
            new Vector3(transform.position.x,
                transform.position.y,
                targetPosition.z);

        if (transform.position.x - targetPosition.x != 0)
        {
            transform.position = 
                new (Mathf
                    .Lerp(transform.position.x, targetPosition.x, 1 - Mathf
                        .Exp(-followResponsiveness * Time.deltaTime)), 
                    transform.position.y, 
                    transform.position.z);
        }

    }
}
