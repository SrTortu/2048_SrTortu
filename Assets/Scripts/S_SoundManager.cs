using UnityEngine;
using UnityEngine.UI;

public class S_SoundManager : MonoBehaviour
{
    [SerializeField] private S_GridManager _gridManager;
    [SerializeField] private Button _muteButton;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _mergeSound;
    [SerializeField] private AudioClip _spawnSound;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] [Range(0f, 1f)] private float _volume = 0.5f;

    private void Awake()
    {
        // Subscribe to events
        if (_gridManager != null)
        {
            _gridManager.OnTileMerge += PlayMergeSound;
            _gridManager.OnTileSpawned += PlaySpawnSound;
        }
        if (_muteButton != null)
        {
            _muteButton.onClick.AddListener(ToggleMute);
        }
    }

    private void PlayMergeSound()
    {
        if (_mergeSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_mergeSound, _volume);
        }
    }

    private void PlaySpawnSound()
    {
        if (_spawnSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_spawnSound, _volume);
        }
    }
    
    private void ToggleMute()
    {
        _audioSource.mute = !_audioSource.mute;
    }
    
}
