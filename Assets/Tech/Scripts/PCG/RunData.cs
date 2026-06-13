using UnityEngine;
/// <summary>
/// Holds data of the run.
/// </summary>
public class RunData
{
    /// <summary>
    /// Seed of the run.
    /// </summary>
    [SerializeField] private int _seed;
    /// <summary>
    /// Current level indicator of the run.
    /// </summary>
    [SerializeField] private int _currentLevel;

    /// <summary>
    /// Accessor of the seed.
    /// </summary>
    public int Seed => _seed;
    /// <summary>
    /// Acessor of current level.
    /// </summary>
    public int CurrentLevel => _currentLevel;

    /// <summary>
    /// Constructor of RunData with the given seed and optional starting level.
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="currentLevel"></param>
    public RunData(int seed, int currentLevel = 1)
    {
        _seed = seed;
        _currentLevel = currentLevel;
    }

    /// <summary>
    /// Advances to the next level incrementing current level.
    /// </summary>
    public void NextLevel()
    {
        _currentLevel++;
    }

    /// <summary>
    /// Returns a deterministic instance for the current level.
    /// The seed is created by combining an original seed with the current level number.
    /// Not ideal but functional.
    /// </summary>
    /// <returns></returns>
    public System.Random GetLevelRandom()
    {
        //Adding initial seed with current level 
        int levelSeed = _seed + _currentLevel;
        //Applying said addition to random
        return new System.Random(levelSeed);
    }
}
