using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ExperienceDisplay : MonoBehaviour
{
    public Slider slider;

    public Image expContainer;
    public Image expBar;

    private Player player;

    public TextMeshProUGUI lvlText;

    [Space]
    [SerializeField] private Transform expGainVFXParent;

    [Space]
    [SerializeField] private AnimationClip expGainClip;
    [SerializeField] private GameObject expGainObject;

    void Start()
    {
        player = FindObjectOfType<Player>();

        player.OnPlayerExperiencePickup += UpdateExp;
        player.OnPlayerLevelUp += SetMaxExp;
        player.OnPlayerLevelUp += SetExp;
        player.OnPlayerLevelUp += SetLvl;

        SetLvl();
        SetExp();
        SetMaxExp();
    }

    private void SetExp()
    {
        slider.value = player.CurrentExp;
    }

    private void UpdateExp()
    {
        slider.value = player.CurrentExp;
        StartCoroutine(SpawnAndDelete(expGainClip.length, expGainObject));
    }

    private void SetMaxExp()
    {
        slider.maxValue = player.ExpToNextLvl;
    }

    private void SetLvl()
    {
        lvlText.text = "Lvl:" + player.CurrentLvl.ToString();
    }

    private IEnumerator SpawnAndDelete(float time, GameObject obj)
    {
        GameObject spawnedObject = Instantiate(obj);
        spawnedObject.transform.SetParent(expGainVFXParent, false);

        yield return new WaitForSecondsRealtime(time);
        Destroy(spawnedObject);
    }
}
