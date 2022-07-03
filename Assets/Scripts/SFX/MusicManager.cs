using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    private List<AudioClip> availableClips;

    private float timeBetweenClips = .5f;

    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        // todo menu options
        source.volume = .25f;

        SelectClip();
    }

    private void DetectAvailableClips()
    {
        availableClips = new List<AudioClip>();
        foreach (AudioClip clip in clips)
        {
            if (source.clip && clip != source.clip) continue;

            availableClips.Add(clip);
        }
    }

    private void SelectClip()
    {
        DetectAvailableClips();

        AudioClip selectedClip = availableClips[Random.Range(0, availableClips.Count)];

        source.clip = selectedClip;
        source.Play();

        StartCoroutine(WaitAndSelectNextClip(source.clip.length + timeBetweenClips));
    }

    IEnumerator WaitAndSelectNextClip(float time)
    {
        yield return new WaitForSeconds(time);
        SelectClip();
    }
}
