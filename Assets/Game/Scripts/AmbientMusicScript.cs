using Assets.Game.Global;
using UnityEngine;

public class AmbientMusicScript : MonoBehaviour
{
    AudioSource ambientMusic;
    void Start()
    {
        ambientMusic = GetComponent<AudioSource>();
        GameSettings.MusicVolumeChanged += UpdateVolume;
        ambientMusic.volume = GameSettings.MusicVolume;
    }

    private void OnDestroy()
    {
        GameSettings.MusicVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(float volume)
    {
        ambientMusic.volume = volume;
    }
}
