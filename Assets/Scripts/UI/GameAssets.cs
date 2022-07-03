using UnityEngine;
using UnityEngine.Audio;
public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public static GameAssets Instance
    {
        get
        {
            if (instance == null) instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return instance;
        }
    }

    public Transform pfDamagePopup;
    public Transform pfLvlUpScreen;
    public Transform pfPauseScreen;
    public Transform pfDeathScreen;
    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sounds sound;
        public AudioClip audioClip;
        public AudioMixerGroup group;
        
    }
}
