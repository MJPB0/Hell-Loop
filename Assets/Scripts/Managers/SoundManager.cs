using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public static class SoundManager
{
    public enum Sounds
    {
        PlayerHit1,
        PlayerHit2,
        PlayerDeath,
        Dodge,
        Healing,
        Healing1,
        LvlUp,
        Powerup,
        Experience,
        Experience1,
        Experience2,
        EnemyHit,
        EnemyHit1,
        EnemyHit2,
        ChestOpen,
        MenuButton,
        WeaponSwap,
    }

    private static Dictionary<Sounds, float> soundTimerDictionary;
    private static GameObject oneShotSoundGameObject;
    private static AudioSource oneShotAudioSource;
    private static AudioMixer mixer;

    public static void Initialize(Player player)
    {
        soundTimerDictionary = new Dictionary<Sounds, float>();
        soundTimerDictionary[Sounds.EnemyHit] = 0f;
        soundTimerDictionary[Sounds.EnemyHit1] = 0f;
        soundTimerDictionary[Sounds.EnemyHit2] = 0f;

        player.OnPlayerTakeDamage += PlayPlayerHitSound;
        player.OnPlayerDeath += PlayPlayerDeathSound;
        player.OnPlayerHeal += PlayPlayerHealingSound;
        player.OnPlayerDash += PlayPlayerDashSound;
        player.OnPlayerExperiencePickup += PlayPlayerExperienceSound;
        player.OnPlayerLevelUp += PlayPlayerLvlUpSound;

        player.GetComponent<PlayerInventory>().OnCurrentWeaponChange += PlayWeaponSwapSound;
    }
    private static bool CanPlaySound(Sounds sound)
    {
        switch (sound)
        {
            case Sounds.EnemyHit:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float enemyHitTimerMax = 0.5f;
                    if (lastTimePlayed + enemyHitTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sounds.EnemyHit1:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float enemyHitTimerMax = 0.5f;
                    if (lastTimePlayed + enemyHitTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sounds.EnemyHit2:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float enemyHitTimerMax = 0.5f;
                    if (lastTimePlayed + enemyHitTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            default:
                return true;
        }
    }
    public static void PlaySound(Sounds sound, float volume)
    {
        if (mixer == null)
        {
            mixer = Resources.Load("MainMixer") as AudioMixer;
        }

        if (CanPlaySound(sound))
        {
            if (oneShotSoundGameObject == null)
            {
                oneShotSoundGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotSoundGameObject.AddComponent<AudioSource>();
                oneShotAudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound), volume);
        }
    }

    private static AudioClip GetAudioClip(Sounds sound)
    {
        foreach(GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }

    private static void PlayPlayerHitSound()
    {
        float volume = 1f;
        if (Random.Range(0, 2) == 0)
        {
            PlaySound(Sounds.PlayerHit1, volume);
        }
        else
        {
            PlaySound(Sounds.PlayerHit2, volume);
        }
    }
    private static void PlayPlayerDeathSound()
    {
        float volume=1f;
        PlaySound(Sounds.PlayerDeath, volume);
    }

    private static void PlayPlayerHealingSound()
    {
        float volume = 1f;
        if (Random.Range(0, 2) == 0)
        {
            PlaySound(Sounds.Healing, volume);
        }
        else
        {
            PlaySound(Sounds.Healing1, volume);
        }
    }
    private static void PlayPlayerDashSound()
    {
        float volume = 1f;
        PlaySound(Sounds.Dodge, volume);
    }

    private static void PlayPlayerExperienceSound()
    {
        float volume = 0.1f;
        PlaySound(Sounds.Experience, volume);
    }
    private static void PlayPlayerLvlUpSound()
    {
        float volume = 0.5f;
        PlaySound(Sounds.LvlUp, volume);
    }

    private static void PlayWeaponSwapSound()
    {
        float volume = 1f;
        PlaySound(Sounds.WeaponSwap, volume);
    }

    public static void PlayEnemyDamageSound()
    {
        float volume = 0.5f;
        int soundIndex = Random.Range(0, 3);
        if (soundIndex == 0)
        {
            PlaySound(Sounds.EnemyHit, volume);
        }
        else if (soundIndex == 1)
        {
            PlaySound(Sounds.EnemyHit1, volume);
        }
        else
        {
            PlaySound(Sounds.EnemyHit2, volume);
        }
    }

    public static void PlayChestOpenSound()
    {
        float volume = 0.25f;
        PlaySound(Sounds.ChestOpen, volume);
    }

    public static void PlayButtonClickSound()
    {
        float volume = 1f;
        PlaySound(Sounds.MenuButton,volume);
    }

}

