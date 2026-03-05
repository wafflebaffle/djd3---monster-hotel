using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    private Vector3 _mousePos;
    private Camera _cam;
    private Mouse _mouse;
    private Vector3 _point;

    private void Start()
    {
        _cam = Camera.main;
        _mouse = Mouse.current;
    }

    private void Update()
    {
        _mousePos = ScreenToWorld();
        
        Vector3 direction = _mousePos - transform.position;
        direction.y = 0;

        if(direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Debug.Log(lookRotation);
            transform.rotation = lookRotation;
        }
    }

    private Vector3 ScreenToWorld()
    {
        RaycastHit hit;
        Ray raycast = _cam.ScreenPointToRay(_mouse.position.ReadValue());

        if (Physics.Raycast(raycast, out hit))
        {
            _point = hit.point;
        }

        return _point;
    }
}
