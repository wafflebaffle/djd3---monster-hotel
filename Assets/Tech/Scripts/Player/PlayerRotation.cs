using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    private Vector3 _mousePos;
    private Camera _cam;
    private Mouse _mouse;
    private CharacterController _controller;
    private void Start()
    {
        _cam = Camera.main;
        _mouse = Mouse.current;
    }

    private void Update()
    {
        _mousePos = _cam.ScreenToWorldPoint(_mouse.position.ReadValue());
        Debug.Log(_mousePos);
        float angle = Vector3.Angle(transform.forward, _mousePos);
        Debug.Log("Angle: " + angle);

        transform.rotation = Quaternion.Euler(0, angle,0);
    }
}
