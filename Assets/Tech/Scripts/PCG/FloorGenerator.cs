using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    [SerializeField] private Transform[] corridorConection;

    private Transform levelRoot;
    private readonly List<Room> generatedRooms = new();

    public void Generate(System.Random random)
    {
        CreateLevelRoot();
        GenerateRoom(random);
    }

    private void GenerateRoom(System.Random random)
    {
        List<Transform> shuffledConnections = ShuffleConnections(random);

        Room[] chosenRooms = new Room[shuffledConnections.Count];
        RoomGenerato roomGen = new RoomGenerato(random);

        for (int i = 0; i < shuffledConnections.Count; i++)
        {
            chosenRooms[i] = roomGen.PickRoom(rooms);
        }

        for (int i = 0; i < shuffledConnections.Count; i++)
        {
            Room roomprefab = chosenRooms[i];
            if (roomprefab != null)
            {
                Room room = Instantiate(roomprefab);
                AttachRoom(room, shuffledConnections[i]);
                generatedRooms.Add(room);
            }
        }
    }


    private void AttachRoom(Room room, Transform conection)
    {
        Transform entrance = room.Entrance;
        room.OriginalQuaternion = room.transform.rotation;

        Quaternion rotation = Quaternion.FromToRotation(entrance.forward, -conection.forward);

        room.transform.rotation = rotation * room.transform.rotation;

        room.transform.position += conection.position - entrance.position;

        generatedRooms.Add(room);

        room.transform.SetParent(levelRoot);
    }

    private List<Transform> ShuffleConnections(System.Random random)
    {
        List<Transform> list = new List<Transform>(corridorConection);

        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = random.Next(i, list.Count);

            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }

        return list;
    }

    private void CreateLevelRoot()
    {        
        levelRoot = new GameObject("LevelRoot").transform;
        levelRoot.transform.SetParent(this.transform);
    }

    public void ClearLevel()
    {
        if (levelRoot != null)
        {
            Destroy(levelRoot.gameObject);
        }

        generatedRooms.Clear();
    }
}
