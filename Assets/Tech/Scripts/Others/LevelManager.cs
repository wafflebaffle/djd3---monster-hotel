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

    private int _enemiesAlive;
    private void EnemyHasSpawn() => _enemiesAlive++;
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
