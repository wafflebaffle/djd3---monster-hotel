using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomNotifier : MonoBehaviour
{
    [SerializeField] private Transform cameraPos, invertedCamPos;
    [SerializeField] private float cameraReturnBreak = 0.5f;
    [SerializeField] private GameObject allMeshes;
    public event Action<bool> OnPlayerEnter;
    private CameraBehaviour _cam;
    private Room _thisRoom;
    private Quaternion _originalQuaternion;

    private void Start()
    {
        _cam = Camera.main.GetComponent<CameraBehaviour>();
        _thisRoom = GetComponent<Room>();
        _originalQuaternion = _thisRoom.OriginalQuaternion;
    }

    /// <summary>
    /// Verify if the player entered a room. If yes, change camera and warn all enemies.
    /// </summary>
    /// <param name="other"> Collider received on enter trigger </param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerStats player))
        {
            allMeshes.SetActive(true);

            OnPlayerEnter?.Invoke(true);

            CancelInvoke(nameof(ReturnToRoamingCamera));

            if (transform.rotation == _originalQuaternion) 
                _cam.ActivateBattleCamera(cameraPos);
            else _cam.ActivateBattleCamera(invertedCamPos);
        }
    }

    /// <summary>
    /// Verify if the player exit a room. If yes, change camera and warn all enemies.
    /// </summary>
    /// <param name="other"> Collider received on exit trigger </param>
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerStats player))
        {
            allMeshes.SetActive(false);

            OnPlayerEnter?.Invoke(false);
            
            Invoke(nameof(ReturnToRoamingCamera), cameraReturnBreak);
        }
    }

    /// <summary>
    /// Return camera to roaming position and rotation.
    /// </summary>
    private void ReturnToRoamingCamera()
    {
        _cam.ActivateRoamingCamera();
    }
}
