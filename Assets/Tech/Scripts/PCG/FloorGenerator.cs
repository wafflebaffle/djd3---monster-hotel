using System;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] roomprefab;
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

        foreach (Transform conection in corridorConection)
        {
            SpawnRoom(conection);
        }
    }

    private void AttachRoom(Room room, Transform conection)
    {
        Transform entrance = room.Entrance;

        //matem quem inventou quaternions, 2 horas nesta brincadeira
        Quaternion rotation = 
            Quaternion.FromToRotation(entrance.forward, -conection.forward);

        room.transform.rotation = rotation * room.transform.rotation;

        Vector3 offset = conection.position - entrance.position;

        room.transform.position += offset;

    }

    private void SpawnRoom(Transform conection)
    {
        int randomIndex = random.Next(0, roomprefab.Length);

        Room room = Instantiate(roomprefab[randomIndex]);

        AttachRoom(room, conection);
    }

}
