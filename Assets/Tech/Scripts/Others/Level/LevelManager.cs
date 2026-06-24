using UnityEngine;
using UnityEngine.Audio;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private EnemyEventChannel enemyEvent;
    [SerializeField] private RandomUpgradeSelector upgradePanel;
    [SerializeField] private MeshRenderer mesh;

    [ Header("Ambient Sound")]
    [SerializeField] private AudioMixerGroup MusicGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip music1;

    [SerializeField] private RunManager runManager;

    private bool _levelCleared;

    private int _enemiesAlive;
    private void EnemyHasSpawn()
    {
        _enemiesAlive++;
        if (!mesh.enabled) mesh.enabled = true;
        if (_levelCleared) _levelCleared = false;
    }
    private void EnemyHasDied()
    {
        _enemiesAlive--;
        if(_enemiesAlive <= 0) DeactivateSeal();
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

    private void Start()
    {
        audioSource.clip = music1;
        audioSource.Play();

        if (audioSource && MusicGroup)
        {
            audioSource.outputAudioMixerGroup = MusicGroup;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_levelCleared) return;
        if (_enemiesAlive > 0) return;

        if (other.TryGetComponent(out PlayerStats player))
        {
            _levelCleared = true;

            if (runManager.CurrentLevel >= runManager.MaxLevels)
            {
                runManager.EndGame();
            }
            else
            {
                player.RemoveBuff();
                upgradePanel.SetPlayer(player);
                upgradePanel.gameObject.SetActive(true);
            }
        }
    }

    private void DeactivateSeal()
    {
        mesh.enabled = false;
    }
}
