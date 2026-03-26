using System;
using UnityEngine;

public class RoomNotifier : MonoBehaviour
{
    public event Action<bool> OnPlayerEnter;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerStats player))
        {
            OnPlayerEnter?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerStats player))
        {
            OnPlayerEnter?.Invoke(false);
        }
    }
}
