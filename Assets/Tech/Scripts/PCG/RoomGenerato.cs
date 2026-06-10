using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerato 
{
    private System.Random random;

    private List<Room> avaliableRooms;


    public RoomGenerato(System.Random random)
    {
        this.random = random;
    }

    //sem repetição de salas só pra testar
    public Room PickRoom(Room[] rooms)
    {
        if (avaliableRooms == null|| avaliableRooms.Count == 0)
        {
            avaliableRooms = new List<Room>(rooms);
        }

        if(avaliableRooms.Count == 0)
        {
            return null;
        }

        int index = random.Next(avaliableRooms.Count);
        Room chosen = avaliableRooms[index];
        avaliableRooms.RemoveAt(index);
        return chosen;
    }
}
