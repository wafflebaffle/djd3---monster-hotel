using UnityEngine;
using UnityEngine.InputSystem;

public class MousePos : MonoBehaviour
{
    private Camera _cam;
    private Mouse _mouse;
    private void Start()
    {
        _cam = Camera.main;
        _mouse = Mouse.current;
    }

    private void Update()
    {
        RaycastHit hit;
        Ray raycast = _cam.ScreenPointToRay(_mouse.position.ReadValue());

        if (Physics.Raycast(raycast, out hit))
        {
            transform.position = hit.point;
        }
    }
}
