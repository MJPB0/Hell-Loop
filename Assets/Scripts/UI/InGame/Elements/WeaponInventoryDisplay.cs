using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponInventoryDisplay : MonoBehaviour
{
    public Image weapon1;
    public Image weapon2;
    public Image weapon3;

    public Image weapon1border;
    public Image weapon2border;
    public Image weapon3border;

    public TextMeshProUGUI weapon1lvl;
    public TextMeshProUGUI weapon2lvl;
    public TextMeshProUGUI weapon3lvl;
    public TextMeshProUGUI weapon1timer;
    public TextMeshProUGUI weapon2timer;
    public TextMeshProUGUI weapon3timer;

    public TextMeshProUGUI replaceText;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button cancelButton;

    private Vector3 scaleChoosen, scaleNotChoosen;

    private PlayerInventory playerInventory;
    private List<PlayerWeapon> playerWeapons;
    private PlayerController playerController;

    public int listSize;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerController = FindObjectOfType<PlayerController>();
        playerWeapons = playerInventory.PlayerWeaponsList;

        scaleChoosen = new Vector3 (2.2f, 2.2f, 0);
        scaleNotChoosen = new Vector3(1.8f, 1.8f, 0);

        playerInventory.OnWeaponPickUp += WeaponsDisplay;
        playerInventory.OnWeaponLevelUp += WeaponsDisplay;
        playerInventory.OnCurrentWeaponChange += CurrentWeaponChange;
        playerInventory.OnCurrentWeaponChange += WeaponsDisplay;
        playerInventory.OnWeaponReplace += WeaponsDisplay;
        playerInventory.UiOnWeaponReplace += UiReplaceWeapon;
        playerInventory.OnWeaponEvolve += UiEvolveWeapon;
    }
    private void Update()
    {
        if (playerWeapons.Count >= 1 && playerWeapons[0].CanAttack)
        {
            weapon1timer.alpha = 0f;
        }
        else if (playerWeapons.Count >= 1 && !playerWeapons[0].CanAttack)
        {
            weapon1timer.alpha = 1f;
            weapon1timer.text = Math.Round(playerWeapons[0].TimeToNextAttack,1).ToString();
        }

        if (playerWeapons.Count >= 2 && playerWeapons[1].CanAttack)
        {
            weapon2timer.alpha = 0f;
        }
        else if (playerWeapons.Count >= 2 && !playerWeapons[1].CanAttack)
        {
            weapon2timer.alpha = 1f;
            weapon2timer.text = Math.Round(playerWeapons[1].TimeToNextAttack, 1).ToString();
        }

        if (playerWeapons.Count == 3 && playerWeapons[2].CanAttack)
        {
            weapon3timer.alpha = 0f;
        }
        else if (playerWeapons.Count == 3 && !playerWeapons[2].CanAttack)
        {
            weapon3timer.alpha = 1f;
            weapon3timer.text = Math.Round(playerWeapons[2].TimeToNextAttack, 1).ToString();
        }
    }

    void WeaponsDisplay()
    {
        playerWeapons = playerInventory.PlayerWeaponsList;
        if (playerWeapons.Count >= 1)
        {
            weapon1.color = new Color(weapon1.color.r, weapon1.color.g, weapon1.color.b, 1f);
            weapon1.sprite = playerWeapons[0].WeaponSprite;
            weapon1lvl.text = "lvl: " + playerWeapons[0].CurrentLevel.ToString() + "/" + playerWeapons[0].MaxLevel.ToString();
        }
        
        if (playerWeapons.Count >= 2)
        {
            weapon2.sprite = playerWeapons[1].WeaponSprite;
            weapon2.color = new Color(weapon2.color.r, weapon2.color.g, weapon2.color.b, 1f);
            weapon2lvl.text = "lvl: " + playerWeapons[1].CurrentLevel.ToString() + "/" + playerWeapons[1].MaxLevel.ToString();
        } 
       
        if (playerWeapons.Count == 3)
        {
            weapon3.sprite = playerWeapons[2].WeaponSprite;
            weapon3.color = new Color(weapon3.color.r, weapon3.color.g, weapon3.color.b, 1f);
            weapon3lvl.text = "lvl: " + playerWeapons[2].CurrentLevel.ToString() + "/" + playerWeapons[2].MaxLevel.ToString();
        }
    }

    void CurrentWeaponChange()
    {
        playerWeapons = playerInventory.PlayerWeaponsList;

        int currentWeaponIndex = playerWeapons.FindIndex((w) => w == playerInventory.CurrentWeapon);

        if (currentWeaponIndex == -1) return;

        switch (currentWeaponIndex)
        {
            case 0:
                weapon1border.transform.localScale = scaleChoosen;
                weapon2border.transform.localScale = scaleNotChoosen;
                weapon3border.transform.localScale = scaleNotChoosen;
                break;
            case 1:
                weapon1border.transform.localScale = scaleNotChoosen;
                weapon2border.transform.localScale = scaleChoosen;
                weapon3border.transform.localScale = scaleNotChoosen;
                break;
            case 2:
                weapon1border.transform.localScale = scaleNotChoosen;
                weapon2border.transform.localScale = scaleNotChoosen;
                weapon3border.transform.localScale = scaleChoosen;
                break;
        }
    }

    void UiReplaceWeapon()
    {
        replaceText.alpha = 1f;
        cancelButton.gameObject.SetActive(true);

        button1.onClick.AddListener(() =>
        {
            playerController.IndexSelected = 0;
            replaceText.alpha = 0f;
            cancelButton.gameObject.SetActive(false);
        });

        button2.onClick.AddListener(() =>
        {
            playerController.IndexSelected = 1;
            replaceText.alpha = 0f;
            cancelButton.gameObject.SetActive(false);
        });

        button3.onClick.AddListener(() =>
        {
            playerController.IndexSelected = 2;
            replaceText.alpha = 0f;
            cancelButton.gameObject.SetActive(false);
        });

        cancelButton.onClick.AddListener(() =>
        {
            playerController.IndexSelected = 3;
            replaceText.alpha = 0f;
            cancelButton.gameObject.SetActive(false);
        });
    }

    void UiEvolveWeapon()
    {
        playerWeapons = playerInventory.PlayerWeaponsList;
        if (playerWeapons.Count >= 1)
        {
            weapon1.color = new Color(weapon1.color.r, weapon1.color.g, weapon1.color.b, 1f);
            weapon1.sprite = playerWeapons[0].WeaponSprite;
            weapon1lvl.text = "lvl: " + playerWeapons[0].CurrentLevel.ToString();

            if (playerWeapons[0].IsEvolved)
            {
                weapon1border.color = Color.yellow;
            }
        }

        if (playerWeapons.Count >= 2)
        {
            weapon2.sprite = playerWeapons[1].WeaponSprite;
            weapon2.color = new Color(weapon2.color.r, weapon2.color.g, weapon2.color.b, 1f);
            weapon2lvl.text = "lvl: " + playerWeapons[1].CurrentLevel.ToString();

            if (playerWeapons[1].IsEvolved)
            {
                weapon2border.color = Color.yellow;
            }
        }

        if (playerWeapons.Count == 3)
        {
            weapon3.sprite = playerWeapons[2].WeaponSprite;
            weapon3.color = new Color(weapon3.color.r, weapon3.color.g, weapon3.color.b, 1f);
            weapon3lvl.text = "lvl: " + playerWeapons[2].CurrentLevel.ToString();

            if (playerWeapons[2].IsEvolved)
            {
                weapon3border.color = Color.yellow;
            }
        }
    }
}
