using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Story Builder/New Speaker", order = 1)]
public class Speaker: ScriptableObject
{
    public Speaker(string name, string portraitPath) {
        this.speakerName = name;
        if (!string.IsNullOrEmpty(portraitPath))
        {
            portrait = Resources.Load<Sprite>("Portraits/" + portraitPath);
            if (portrait == null)
            {
                Debug.LogError("Portrait for " + name + " not found at " + portraitPath);
            }
        }
    }

    public override string ToString() {
        return speakerName;
    }

    public string speakerName;
    public string fullName;
    public List<TMPro.TMP_FontAsset> fonts = new List<TMPro.TMP_FontAsset>();
    public Color nametagColor;
    public Sprite portrait;
    public GameObject assetPrefab;
    public List<Sprite> emotions;
    public List<AudioClip> audioBarks;

    public Sprite nameCard;

    [Tooltip("Sounds that play per text letter typed")]
    public List<AudioClip> pips;

    public AudioClip chatter;

    [Space]
    public float characterScale = 0.65f;


    [System.Serializable]
    public struct PortraitState {
        public string name;
        public Sprite portrait;
    }
    [System.Serializable]
    public struct ExtraData {
        public string name;
        public string data;
    }
    public ExtraData[] additionalData;
}

