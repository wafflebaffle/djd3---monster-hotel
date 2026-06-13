using System.Collections.Generic;

/// <summary>
/// Picks rooms from a pool of options.
/// </summary>
public class RoomGenerator 
{
    /// <summary>
    /// Source of random, created via constructor.
    /// </summary>
    private System.Random random;

    /// <summary>
    /// List of rooms still avaliable.
    /// </summary>
    private List<Room> avaliableRooms;

    /// <summary>
    /// Creates a room picker that uses the given random.
    /// </summary>
    /// <param name="random"></param>
    public RoomGenerator(System.Random random)
    {
        this.random = random;
    }

    /// <summary>
    /// Returns a random room from the array.
    /// </summary>
    /// <param name="rooms"></param>
    /// <returns></returns>
    public Room PickRoom(Room[] rooms)
    {
        // if the avaliable is null or empty refill it with rooms.
        if (avaliableRooms == null|| avaliableRooms.Count == 0)
        {
            avaliableRooms = new List<Room>(rooms);
        }

        // if no rooms could be added return null.
        if(avaliableRooms.Count == 0)
        {
            return null;
        }

        // Select a random index from the avaliable rooms.
        int index = random.Next(avaliableRooms.Count);
        Room chosen = avaliableRooms[index];

        //Remove the chosen room so it isnt picked again until the list is refiled.
        avaliableRooms.RemoveAt(index);
        return chosen;
    }
}
