public interface ISaveable
{
    public string GetSaveID();

    public object GetSaveData();

    public void LoadSaveData(object data);
}
