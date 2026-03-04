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
        transform.position = _cam.ScreenToWorldPoint(_mouse.position.ReadValue());
    }
}
