using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Generates the floor layout by instanciating and attaching them to the predefined connection.
/// </summary>
public class FloorGenerator : MonoBehaviour
{
    /// <summary>
    /// Pool of room prefabs to choose.
    /// </summary>
    [SerializeField] private Room[] rooms;
    /// <summary>
    /// Connection points where the rooms attach.
    /// </summary>
    [SerializeField] private Transform[] corridorConection;

    /// <summary>
    /// Parent of the rooms.
    /// </summary>
    private Transform levelRoot;
    /// <summary>
    /// List of rooms created.
    /// </summary>
    private readonly List<Room> generatedRooms = new();

    /// <summary>
    /// Starts generating rooms inside the level root.
    /// </summary>
    /// <param name="random"></param>
    public void Generate(System.Random random)
    {
        CreateLevelRoot();
        GenerateRoom(random);
    }

    /// <summary>
    /// Shuffles the connection points and then picks a random room for each.
    /// </summary>
    /// <param name="random"></param>
    private void GenerateRoom(System.Random random)
    {
        // Shuffling the connections.
        List<Transform> shuffledConnections = ShuffleConnections(random);

        // Array holding the room chosen.
        Room[] chosenRooms = new Room[shuffledConnections.Count];
        // Room picker
        RoomGenerator roomGen = new RoomGenerator(random);

        // Pick a room fro each connection point.
        for (int i = 0; i < shuffledConnections.Count; i++)
        {
            chosenRooms[i] = roomGen.PickRoom(rooms);
        }

        // Instantiante and attach.
        for (int i = 0; i < shuffledConnections.Count; i++)
        {
            Room roomprefab = chosenRooms[i];
            if (roomprefab != null)
            {
                // Instantiate the room prefab.
                Room room = Instantiate(roomprefab);
                // Position and rotation for specific connection.
                AttachRoom(room, shuffledConnections[i]);
                // Keep rooms already generated.
                generatedRooms.Add(room);
            }
        }
    }

    /// <summary>
    /// Positions and rotates a room so that its entrance aligns with the given connection point.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="conection"></param>
    private void AttachRoom(Room room, Transform conection)
    {
        // The rooms entrance.
        Transform entrance = room.Entrance;
        // Original rotations. 
        room.OriginalQuaternion = room.transform.rotation;

        // Calculations to find the rottation needed to aline entrance and connection.
        Quaternion rotation = Quaternion.FromToRotation(entrance.forward, -conection.forward);

        // Applying the rotation to the rooms.
        room.transform.rotation = rotation * room.transform.rotation;

        // Moving the room so that the entrance aligns exactly with the connection.
        room.transform.position += conection.position - entrance.position;

        // Parent the room under the level root.
        generatedRooms.Add(room);
        room.transform.SetParent(levelRoot);
    }

    /// <summary>
    /// Shuffling connections with Fisher-Yates.
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    private List<Transform> ShuffleConnections(System.Random random)
    {
        List<Transform> list = new List<Transform>(corridorConection);

        // Standart Fisher-Yates algoritm.
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = random.Next(i, list.Count);

            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }

        return list;
    }

    /// <summary>
    /// Create empty gameobject "LevelRoot".
    /// </summary>
    private void CreateLevelRoot()
    {        
        levelRoot = new GameObject("LevelRoot").transform;
        levelRoot.transform.SetParent(this.transform);
    }

    /// <summary>
    /// Destroys the entire level hierarchy and clears the generated rooms list.
    /// </summary>
    public void ClearLevel()
    {
        if (levelRoot != null)
        {
            Destroy(levelRoot.gameObject);
        }

        generatedRooms.Clear();
    }
}
