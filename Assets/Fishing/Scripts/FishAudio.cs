using UnityEngine;
using UnityEngine.Audio;

public class FishAudio : MonoBehaviour
{
    public AudioMixerGroup target;

    public void OnEnable()
    {
        target.audioMixer.SetFloat("Volume", 0);
    }

    private void OnDisable()
    {
        target.audioMixer.SetFloat("Volume", -80);
    }
}
