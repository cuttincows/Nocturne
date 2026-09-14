using UnityEngine;
using UnityEngine.Audio;

public class FishAudio : MonoBehaviour
{
    public AudioMixerGroup target;
    public float audibleVolume = 0f;
    public float mutedVolume = -80f;

    private void Awake()
    {
        Mute(true);
    }

    public void OnEnable()
    {
        Mute(false);
    }

    private void OnDisable()
    {
        Mute(true);
    }

    public void Mute(bool muted)
    {
        if (target == null)
        {
            return;
        }

        target.audioMixer.SetFloat("Volume", muted ? mutedVolume : audibleVolume);
    }
}
