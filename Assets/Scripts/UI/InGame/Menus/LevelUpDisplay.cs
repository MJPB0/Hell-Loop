using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelUpDisplay : MonoBehaviour
{
    private ICollection<Transform> componentsList;
    private PlayerController playerController;

    public void Awake()
    {
        componentsList = transform.GetComponentsInChildren<Transform>();
        playerController = FindObjectOfType<PlayerController>();
    }

    public static void Create(List<PlayerPassive> choices, List<PlayerPassive>playerPassives)
    {
        //TODO case when less than 3
        Canvas parent = FindObjectOfType<Canvas>();
        Transform lvlUpDisplayTransform = Instantiate(GameAssets.Instance.pfLvlUpScreen, Vector3.zero, Quaternion.identity, parent.transform);
        lvlUpDisplayTransform.localPosition = new(0, 75, 0);
        
        LevelUpDisplay lvlUpDisplay = lvlUpDisplayTransform.GetComponent<LevelUpDisplay>();

        lvlUpDisplay.Setup(choices, playerPassives);
    }

    public void Setup(List<PlayerPassive> choices, List<PlayerPassive> playerPassives)
    {
        if (choices.Count == 0 && choices.Count != 2 && choices.Count != 3 && choices.Count != 1)
        {
            playerController.IndexSelected = 3;
            Destroy(gameObject);
        }

        if (choices.Count == 1 && choices.Count != 2 && choices.Count != 3 && choices.Count != 0)
        {
            foreach (var child in componentsList)
            {
                if (child.name == "perk2Holder")
                {
                    child.gameObject.SetActive(false);
                }
                if (child.name == "perk3Holder")
                {
                    child.gameObject.SetActive(false);
                }

                if (child.name == "perk1SpriteImage")
                {
                    child.GetComponent<Image>().sprite = choices[0].PassiveSprite;
                }
                else if (child.name == "perk1lvltext")
                {
                    List<string> passivenames = new List<string>();

                    foreach (var passive in playerPassives)
                    {
                        passivenames.Add(passive.PassiveName.ToString());
                    }

                    if (passivenames.Contains(choices[0].PassiveName.ToString()))
                    {
                        if (playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].CurrentLevel + 1 == playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].MaxLevel)
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "MAX";
                        }
                        child.GetComponent<TextMeshProUGUI>().text = "lvl: " + (playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].CurrentLevel + 1).ToString();
                    }
                    else
                    {
                        child.GetComponent<TextMeshProUGUI>().text = "NEW";
                    }

                    passivenames.Clear();
                }
                else if (child.name == "perk1DescriptionText")
                {
                    child.GetComponent<TextMeshProUGUI>().text = choices[0].PassiveName.ToString().ToLower().Replace("_", " ") + ": " + choices[0].PassiveDescription + " " + (choices[0].NextUpgradeValue * 100).ToString() + "%";
                }
                else if (child.name == "perk1Button")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SoundManager.PlayButtonClickSound();
                        playerController.IndexSelected = 0;
                        Destroy(gameObject);
                    });
                }
            }
        }

        if (choices.Count == 2 && choices.Count != 1 && choices.Count != 3 && choices.Count != 0)
        {
            foreach (var child in componentsList)
            {
                if (child.name == "perk3Holder")
                {
                    child.gameObject.SetActive(false);
                }

                if (child.name == "perk1SpriteImage")
                {
                    child.GetComponent<Image>().sprite = choices[0].PassiveSprite;
                }
                else if (child.name == "perk1lvltext")
                {
                    List<string> passivenames = new List<string>();

                    foreach (var passive in playerPassives)
                    {
                        passivenames.Add(passive.PassiveName.ToString());
                    }

                    if (passivenames.Contains(choices[0].PassiveName.ToString()))
                    {
                        if (playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].CurrentLevel + 1 == playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].MaxLevel)
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "MAX";
                        }
                        child.GetComponent<TextMeshProUGUI>().text = "lvl: " + (playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].CurrentLevel + 1).ToString();
                    }
                    else
                    {
                        child.GetComponent<TextMeshProUGUI>().text = "NEW";
                    }

                    passivenames.Clear();
                }
                else if (child.name == "perk1DescriptionText")
                {
                    child.GetComponent<TextMeshProUGUI>().text = choices[0].PassiveName.ToString().ToLower().Replace("_", " ") + ": " + choices[0].PassiveDescription + " " + (choices[0].NextUpgradeValue * 100).ToString() + "%";
                }
                else if (child.name == "perk1Button")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SoundManager.PlayButtonClickSound();
                        playerController.IndexSelected = 0;
                        Destroy(gameObject);
                    });
                }
                else if (child.name == "perk2SpriteImage")
                {
                    child.GetComponent<Image>().sprite = choices[1].PassiveSprite;
                }
                else if (child.name == "perk2lvltext")
                {
                    List<string> passivenames = new List<string>();

                    foreach (var passive in playerPassives)
                    {
                        passivenames.Add(passive.PassiveName.ToString());
                    }

                    if (passivenames.Contains(choices[1].PassiveName.ToString()))
                    {
                        if (playerPassives[passivenames.IndexOf(choices[1].PassiveName.ToString())].CurrentLevel + 1 == playerPassives[passivenames.IndexOf(choices[1].PassiveName.ToString())].MaxLevel)
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "MAX";
                        }
                        child.GetComponent<TextMeshProUGUI>().text = "lvl: " + (playerPassives[passivenames.IndexOf(choices[1].PassiveName.ToString())].CurrentLevel + 1).ToString();
                    }
                    else
                    {
                        child.GetComponent<TextMeshProUGUI>().text = "NEW";
                    }

                    passivenames.Clear();
                }
                else if (child.name == "perk2DescriptionText")
                {
                    child.GetComponent<TextMeshProUGUI>().text = choices[1].PassiveName.ToString().ToLower().Replace("_", " ") + ": " + choices[1].PassiveDescription + " " + (choices[1].NextUpgradeValue * 100).ToString() + "%";
                }
                else if (child.name == "perk2Button")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SoundManager.PlayButtonClickSound();
                        playerController.IndexSelected = 1;
                        Destroy(gameObject);
                    });
                }
            }
        }

        if (choices.Count == 3 && choices.Count != 2 && choices.Count != 1 && choices.Count != 0)
        {
            foreach (var child in componentsList)
            {
                if (child.name == "perk1SpriteImage")
                {
                    child.GetComponent<Image>().sprite = choices[0].PassiveSprite;
                }
                else if (child.name == "perk1lvltext")
                {
                    List<string> passivenames = new List<string>();

                    foreach (var passive in playerPassives)
                    {
                        passivenames.Add(passive.PassiveName.ToString());
                    }

                    if (passivenames.Contains(choices[0].PassiveName.ToString()))
                    {
                        if (playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].CurrentLevel + 1 == playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].MaxLevel)
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "MAX";
                        }
                        child.GetComponent<TextMeshProUGUI>().text = "lvl: " + (playerPassives[passivenames.IndexOf(choices[0].PassiveName.ToString())].CurrentLevel + 1).ToString();
                    }
                    else
                    {
                        child.GetComponent<TextMeshProUGUI>().text = "NEW";
                    }

                    passivenames.Clear();
                }
                else if (child.name == "perk1DescriptionText")
                {
                    child.GetComponent<TextMeshProUGUI>().text = choices[0].PassiveName.ToString().ToLower().Replace("_", " ") + ": " + choices[0].PassiveDescription + " " + (choices[0].NextUpgradeValue * 100).ToString() + "%";
                }
                else if (child.name == "perk1Button")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SoundManager.PlayButtonClickSound();
                        playerController.IndexSelected = 0;
                        Destroy(gameObject);
                    });
                }
                else if (child.name == "perk2SpriteImage")
                {
                    child.GetComponent<Image>().sprite = choices[1].PassiveSprite;
                }
                else if (child.name == "perk2lvltext")
                {
                    List<string> passivenames = new List<string>();

                    foreach (var passive in playerPassives)
                    {
                        passivenames.Add(passive.PassiveName.ToString());
                    }

                    if (passivenames.Contains(choices[1].PassiveName.ToString()))
                    {
                        if (playerPassives[passivenames.IndexOf(choices[1].PassiveName.ToString())].CurrentLevel + 1 == playerPassives[passivenames.IndexOf(choices[1].PassiveName.ToString())].MaxLevel)
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "MAX";
                        }
                        child.GetComponent<TextMeshProUGUI>().text = "lvl: " + (playerPassives[passivenames.IndexOf(choices[1].PassiveName.ToString())].CurrentLevel + 1).ToString();
                    }
                    else
                    {
                        child.GetComponent<TextMeshProUGUI>().text = "NEW";
                    }

                    passivenames.Clear();
                }
                else if (child.name == "perk2DescriptionText")
                {
                    child.GetComponent<TextMeshProUGUI>().text = choices[1].PassiveName.ToString().ToLower().Replace("_", " ") + ": " + choices[1].PassiveDescription + " " + (choices[1].NextUpgradeValue * 100).ToString() + "%";
                }
                else if (child.name == "perk2Button")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SoundManager.PlayButtonClickSound();
                        playerController.IndexSelected = 1;
                        Destroy(gameObject);
                    });
                }
                else if (child.name == "perk3SpriteImage")
                {
                    child.GetComponent<Image>().sprite = choices[2].PassiveSprite;
                }
                else if (child.name == "perk3lvltext")
                {
                    List<string> passivenames = new List<string>();

                    foreach (var passive in playerPassives)
                    {
                        passivenames.Add(passive.PassiveName.ToString());
                    }

                    if (passivenames.Contains(choices[2].PassiveName.ToString()))
                    {
                        if (playerPassives[passivenames.IndexOf(choices[2].PassiveName.ToString())].CurrentLevel + 1 == playerPassives[passivenames.IndexOf(choices[2].PassiveName.ToString())].MaxLevel)
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "MAX";
                        }
                        child.GetComponent<TextMeshProUGUI>().text = "lvl: " + (playerPassives[passivenames.IndexOf(choices[2].PassiveName.ToString())].CurrentLevel + 1).ToString();
                    }
                    else
                    {
                        child.GetComponent<TextMeshProUGUI>().text = "NEW";
                    }

                    passivenames.Clear();
                }
                else if (child.name == "perk3DescriptionText")
                {
                    child.GetComponent<TextMeshProUGUI>().text = choices[2].PassiveName.ToString().ToLower().Replace("_", " ") + ": " + choices[2].PassiveDescription + " " + (choices[2].NextUpgradeValue * 100).ToString() + "%";
                }
                else if (child.name == "perk3Button")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SoundManager.PlayButtonClickSound();
                        playerController.IndexSelected = 2;
                        Destroy(gameObject);
                    });
                }
            }
        }
    }
}
