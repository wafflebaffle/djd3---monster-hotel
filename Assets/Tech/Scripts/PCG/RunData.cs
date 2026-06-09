public class RunData
{
    public int Seed { get; private set; }
    public int CurrentLevel { get; private set; }

    public RunData(int seed)
    {
        Seed = seed;
        CurrentLevel = 1;
    }

    public void NextLevel()
    {
        CurrentLevel++;
    }
}
