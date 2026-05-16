using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    [SerializeField] private Transform[] corridorConection;

    [SerializeField] private bool randomSeed = true;
    [SerializeField] private int seed;
    private System.Random random;

    private void Start()
    {
        if (randomSeed)
        {
            seed = DateTime.Now.GetHashCode();
        }

        Debug.Log("Seed: " + seed);

        random = new System.Random(seed);

        GenerateRoom();
    }

    private void GenerateRoom()
    {
        for (int i = 0; i < corridorConection.Length; i++) 
        {
            Transform connection = corridorConection[i];
            SpawnRoom(connection, i);
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

    private void SpawnRoom(Transform conection, int roomIndex)
    {
        Room roomPrefab = GetRandomRoom(roomIndex);

        Room room = Instantiate(roomPrefab);

        AttachRoom(room, conection);
    }

    private Room GetRandomRoom(int index)
    {
        Room.RoomType wantedType;

        if (index < 2)
        {
            wantedType = Room.RoomType.Small;
        }
        else
        {
            wantedType = Room.RoomType.Big;
        }

        List<Room> validRooms = new List<Room>();

        foreach (Room room in rooms)
        {
            if(room.Type == wantedType)
            {
                validRooms.Add(room);
            }
        }

        return validRooms[random.Next(validRooms.Count)];

    }

}
