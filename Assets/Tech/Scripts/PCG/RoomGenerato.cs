using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerato : MonoBehaviour
{
    private System.Random random;

    public RoomGenerato(int seed)
    {
        random = new System.Random(seed);
    }

    public Room PickRoom(Room[] rooms, int index, int total)
    {
        float progress = index / (float)total;

        List<Room> candidates = new List<Room>();

        foreach (Room room in rooms)
        {
            //regra + add
        }

        return candidates[random.Next(candidates.Count)];
    }

}
