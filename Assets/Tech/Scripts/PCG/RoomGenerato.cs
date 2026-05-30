using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerato 
{
    private System.Random random;

    private HashSet<Room> usedRooms = new HashSet<Room>();


    public RoomGenerato(int seed)
    {
        random = new System.Random(seed);
    }

    public Room PickRoom(Room[] rooms)
    {
        List<Room> validRooms = new List<Room>();

        foreach (Room room in rooms)
        {
            //regra de salas n�o se podem repetir
           //if(!usedRooms.Contains(room))
           //{
                validRooms.Add(room);
           //}
        }

        //fallback
        if (validRooms.Count == 0)
        {
            return null;
        }

        Room chosen = validRooms[random.Next(validRooms.Count)];

        usedRooms.Add(chosen);

        return chosen;
    }

}
