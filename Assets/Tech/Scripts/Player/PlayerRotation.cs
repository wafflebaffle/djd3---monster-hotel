using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    private Camera _cam;
    private Mouse _mouse;
    private Vector3 _point;
    private PlayerStats _stats;

    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _cam = Camera.main;
        _mouse = Mouse.current;
    }

    private void Update()
    {        
        Vector3 direction = ScreenToWorld();

        if(direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 
                                                  _stats.AngularSpeed * Time.deltaTime);;
        }
    }

    private Vector3 ScreenToWorld()
    {
        Vector3 aimDirection = Vector3.zero;

        Plane groundPlane = new Plane(Vector3.up, transform.position);
        Ray raycast = _cam.ScreenPointToRay(_mouse.position.ReadValue());

        if (groundPlane.Raycast(raycast, out float distance))
        {
            Vector3 hitPoint = raycast.GetPoint(distance);
            aimDirection = (hitPoint - transform.position).normalized;
            aimDirection.y = 0;
        }

        return aimDirection;
    }
}
