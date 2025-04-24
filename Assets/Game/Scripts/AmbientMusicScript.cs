using Assets.Game.Global;
using UnityEngine;

public class AmbientMusicScript : MonoBehaviour
{
    AudioSource ambientMusic;
    void Start()
    {
        ambientMusic = GetComponent<AudioSource>();
        ambientMusic.volume = GameSettings.MusicVolume;
        GameSettings.MusicVolumeChanged += UpdateVolume;
    }

    private void OnDestroy()
    {
        GameSettings.MusicVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(float volume)
    {
        Debug.Log(volume);
        ambientMusic.volume = volume;
    }
}
