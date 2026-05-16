using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room[] roomprefab;
    [SerializeField] private Transform[] corridorConection;
    [SerializeField] private int seed;

    private void Start()
    {
        Random.InitState(seed);

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
        int randomIndex = Random.Range(0, roomprefab.Length);

        Room room = Instantiate(roomprefab[randomIndex]);

        AttachRoom(room, conection);
    }

}
