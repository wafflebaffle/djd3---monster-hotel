using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    [SerializeField] private Transform[] corridorConection;
    private System.Random random;

    private RoomGenerato roomGenerator;

    private void Start()
    {
        int seed = RunManager.Seed;

        random = new System.Random(seed);

        roomGenerator = new RoomGenerato(seed);

        GenerateRoom();
    }
    
    private void GenerateRoom()
    {
        List<Transform> shuffledConnections = ShuffleConnections();

        for (int i = 0; i < shuffledConnections.Count; i++) 
        {
            Transform connection = shuffledConnections[i];

            Room roomPrefab = roomGenerator.PickRoom(rooms);

            if (roomPrefab == null)
            {
                Debug.Log("No more rooms avaliable");
                break;
            }

            Room room = Instantiate(roomPrefab);

            AttachRoom(room, connection);
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

    private List<Transform> ShuffleConnections()
    {
        List<Transform> list = new List<Transform>(corridorConection);

        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = random.Next(i, list.Count);

            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }

        return list;
    }

}
