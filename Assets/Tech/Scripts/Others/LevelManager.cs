using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private EnemyEventChannel enemyEvent;
    [SerializeField] private RandomUpgradeSelector upgradePanel;
    [SerializeField] private MeshRenderer mesh;
    [Header("Music Ambient")]
    [SerializeField] private AudioSource universalAudioSource;
    [SerializeField] private AudioClip ambientMusic;
    private int _enemiesAlive;
    private void EnemyHasSpawn() => _enemiesAlive++;
    private void EnemyHasDied()
    {
        _enemiesAlive--;
        if(_enemiesAlive <= 0) DeactivateSeal();
    }

    private void Start()
    {
        universalAudioSource.clip = ambientMusic;
        universalAudioSource.Play();
    }

    private void OnEnable()
    {
        enemyEvent.OnEnemySpawned += EnemyHasSpawn;
        enemyEvent.OnEnemyDied += EnemyHasDied;
    }

    private void OnDisable()
    {
        enemyEvent.OnEnemySpawned -= EnemyHasSpawn;
        enemyEvent.OnEnemyDied -= EnemyHasDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_enemiesAlive <= 0)
            if(other.TryGetComponent(out PlayerStats player))
            {
                player.RemoveBuff();
                upgradePanel.SetPlayer(player);
                upgradePanel.gameObject.SetActive(true);
            }
    }

    private void DeactivateSeal()
    {
        mesh.enabled = false;
    }
}
