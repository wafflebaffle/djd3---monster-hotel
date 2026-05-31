using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    [SerializeField] private Transform[] corridorConection;

    private System.Random random;
    private NavMeshSurface _surfaceAI;

    private RoomGenerato roomGenerator;

    private Transform levelRoot;
    private readonly List<Room> generatedRooms = new();
    

    private void Awake()
    {
        _surfaceAI = GetComponent<NavMeshSurface>();
    }

    public void Generate(int seed)
    {
        random = new System.Random(seed);
        roomGenerator = new RoomGenerato(seed);

        CreateLevelRoot();
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
                break;
            }

            Room room = Instantiate(roomPrefab);

            AttachRoom(room, connection);
        }

        _surfaceAI.BuildNavMesh();
    }


    private void AttachRoom(Room room, Transform conection)
    {
        Transform entrance = room.Entrance;
        room.OriginalQuaternion = room.transform.rotation;

        Quaternion rotation = Quaternion.FromToRotation(entrance.forward, -conection.forward);

        room.transform.rotation = rotation * room.transform.rotation;

        room.transform.position += conection.position - entrance.position;

        room.transform.SetParent(conection);
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

    private void CreateLevelRoot()
    {        
        levelRoot = new GameObject("LevelRoot").transform;
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
