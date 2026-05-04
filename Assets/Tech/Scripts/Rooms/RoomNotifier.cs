using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomNotifier : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;
    [SerializeField] private float cameraReturnBreak = 0.5f;
    [SerializeField] private GameObject allMeshes;
    public event Action<bool> OnPlayerEnter;
    private CameraBehaviour _cam;
    private Vector3 _cameraDestination;

    private void Start()
    {
        _cam = Camera.main.GetComponent<CameraBehaviour>();
        _cameraDestination = cameraPos.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerStats player))
        {
            allMeshes.SetActive(true);

            OnPlayerEnter?.Invoke(true);

            CancelInvoke(nameof(ReturnToRoamingCamera));

            _cam.ActivateBattleCamera(_cameraDestination);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerStats player))
        {
            allMeshes.SetActive(false);

            OnPlayerEnter?.Invoke(false);
            
            Invoke(nameof(ReturnToRoamingCamera), cameraReturnBreak);
        }
    }
    
    private void ReturnToRoamingCamera()
    {
        _cam.ActivateRoamingCamera();
    }
}
