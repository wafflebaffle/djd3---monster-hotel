using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private Room roomprefab;
    [SerializeField] private Transform corridorConection;

    private void Start()
    {
        SpawnRoom();
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

    private void SpawnRoom()
    {
        Room room = Instantiate(roomprefab);

        AttachRoom(room, corridorConection);
    }

}
