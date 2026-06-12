public interface ISaveable
{
    public string GetSaveID();

    public string GetSaveDataAsJson();

    public void LoadFromJson(string json);
}
