using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    [SerializeField] private Transform[] corridorConection;

    private RoomGenerato roomGenerator;

    private void Start()
    {
        int seed = RunManager.Seed;

        roomGenerator = new RoomGenerato(seed);

        GenerateRoom();
    }

    private void GenerateRoom()
    {
        for (int i = 0; i < corridorConection.Length; i++) 
        {
            Room roomPrefab = roomGenerator.PickRoom(rooms);

            if (roomPrefab == null)
            {
                Debug.Log("No more rooms avaliable");
                break;
            }

            Room room = Instantiate(roomPrefab);

            AttachRoom(room, corridorConection[i]);
        }
    }


    private void AttachRoom(Room room, Transform conection)
    {
        Transform entrance = room.Entrance;

        //matem quem inventou quaternions, 2 horas nesta brincadeira
        Quaternion rotation = Quaternion.FromToRotation(entrance.forward, -conection.forward);

        room.transform.rotation = rotation * room.transform.rotation;

        room.transform.position += conection.position - entrance.position;

    }
}
