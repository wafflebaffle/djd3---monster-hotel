using UnityEngine;

public class RunData
{
    [SerializeField] private int _seed;
    [SerializeField] private int _currentLevel;

    public int Seed => _seed;
    public int CurrentLevel => _currentLevel;

    public RunData(int seed, int currentLevel)
    {
        _seed = seed;
        _currentLevel = 1;
    }

    public void NextLevel()
    {
        _currentLevel++;
    }

    public System.Random GetLevelRandom()
    {
        int levelSeed = _seed + _currentLevel;
        return new System.Random(levelSeed);
    }
}
