public class RunData
{
    public int Seed { get; private set; }
    public int CurrentLevel { get; private set; }
    public System.Random Random { get; private set; }

    public RunData(int seed)
    {
        Seed = seed;
        CurrentLevel = 1;
        Random = new System.Random(seed);
    }

    public void NextLevel()
    {
        CurrentLevel++;
    }
}
