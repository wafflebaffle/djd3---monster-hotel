/// <summary>
/// Defines a contract for objects that support saving and loading their state as JSON.
/// </summary>
/// <remarks>Implementations should provide logic for serializing and deserializing object state, as well as a
/// unique identifier for saved data.</remarks>
public interface ISaveable
{
    public string GetSaveID();

    public string GetSaveDataAsJson();

    public void LoadFromJson(string json);
}
