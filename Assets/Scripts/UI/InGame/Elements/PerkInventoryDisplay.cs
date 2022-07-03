using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PerkInventoryDisplay : MonoBehaviour
{
    public Image perk1;
    public Image perk2;
    public Image perk3;
    public Image perk4;
    public Image perk5;

    public TextMeshProUGUI perk1lvl;
    public TextMeshProUGUI perk2lvl;
    public TextMeshProUGUI perk3lvl;
    public TextMeshProUGUI perk4lvl;
    public TextMeshProUGUI perk5lvl;

    public Color color;
    private PlayerInventory playerInventory;
    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerInventory.OnPassiveAdd += PerksDisplay;
        playerInventory.OnPassiveLevelUp += PerksDisplay;
    }
    public void PerksDisplay()
    {
        List<PlayerPassive> playerPerks = playerInventory.PlayerPassivesList;
        if (playerPerks.Count == 1)
        {
            perk1.sprite = playerPerks[0].PassiveSprite;
            perk1.color = new Color(perk1.color.r, perk1.color.g, perk1.color.b, 1f);
            perk1lvl.text = "lvl: " + playerPerks[0].CurrentLevel + "/" + playerPerks[0].MaxLevel.ToString();
        }
        else if (playerPerks.Count == 2)
        {
            perk1.sprite = playerPerks[0].PassiveSprite;
            perk2.sprite = playerPerks[1].PassiveSprite;
            perk2.color = new Color(perk1.color.r, perk1.color.g, perk1.color.b, 1f);
            perk1lvl.text = "lvl: " + playerPerks[0].CurrentLevel + "/" + playerPerks[0].MaxLevel.ToString();
            perk2lvl.text = "lvl: " + playerPerks[1].CurrentLevel + "/" + playerPerks[1].MaxLevel.ToString();
        }
        else if (playerPerks.Count == 3)
        {
            perk1.sprite = playerPerks[0].PassiveSprite;
            perk2.sprite = playerPerks[1].PassiveSprite;
            perk3.sprite = playerPerks[2].PassiveSprite;
            perk3.color = new Color(perk1.color.r, perk1.color.g, perk1.color.b, 1f);
            perk1lvl.text = "lvl: " + playerPerks[0].CurrentLevel + "/" + playerPerks[0].MaxLevel.ToString();
            perk2lvl.text = "lvl: " + playerPerks[1].CurrentLevel + "/" + playerPerks[1].MaxLevel.ToString();
            perk3lvl.text = "lvl: " + playerPerks[2].CurrentLevel + "/" + playerPerks[2].MaxLevel.ToString();
        }
        else if (playerPerks.Count == 4)
        {
            perk1.sprite = playerPerks[0].PassiveSprite;
            perk2.sprite = playerPerks[1].PassiveSprite;
            perk3.sprite = playerPerks[2].PassiveSprite;
            perk4.sprite = playerPerks[3].PassiveSprite;
            perk4.color = new Color(perk1.color.r, perk1.color.g, perk1.color.b, 1f);
            perk1lvl.text = "lvl: " + playerPerks[0].CurrentLevel + "/" + playerPerks[0].MaxLevel.ToString();
            perk2lvl.text = "lvl: " + playerPerks[1].CurrentLevel + "/" + playerPerks[1].MaxLevel.ToString();
            perk3lvl.text = "lvl: " + playerPerks[2].CurrentLevel + "/" + playerPerks[2].MaxLevel.ToString();
            perk4lvl.text = "lvl: " + playerPerks[3].CurrentLevel + "/" + playerPerks[3].MaxLevel.ToString();
        }
        else if (playerPerks.Count == 5)
        {
            perk1.sprite = playerPerks[0].PassiveSprite;
            perk2.sprite = playerPerks[1].PassiveSprite;
            perk3.sprite = playerPerks[2].PassiveSprite;
            perk4.sprite = playerPerks[3].PassiveSprite;
            perk5.sprite = playerPerks[4].PassiveSprite;
            perk5.color = new Color(perk1.color.r, perk1.color.g, perk1.color.b, 1f);
            perk1lvl.text = "lvl: " + playerPerks[0].CurrentLevel + "/" + playerPerks[0].MaxLevel.ToString();
            perk2lvl.text = "lvl: " + playerPerks[1].CurrentLevel + "/" + playerPerks[1].MaxLevel.ToString();
            perk3lvl.text = "lvl: " + playerPerks[2].CurrentLevel + "/" + playerPerks[2].MaxLevel.ToString();
            perk4lvl.text = "lvl: " + playerPerks[3].CurrentLevel + "/" + playerPerks[3].MaxLevel.ToString();
            perk5lvl.text = "lvl: " + playerPerks[4].CurrentLevel + "/" + playerPerks[4].MaxLevel.ToString();
        }
    }
}
