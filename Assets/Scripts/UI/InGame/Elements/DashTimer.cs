using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class DashTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image dashIcon;
    private Color colorCan = new Color(1f,1f,1f, 0f);
    private Color colorCant = new Color(1f, 1f, 1f, 1f);

    private Player player;
    public void SetTime()
    {
        timerText.text = Math.Round(player.TimeToNextDash, 1).ToString();
    }
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    void Update()
    {
        if (player.CanDash)
        {
            timerText.color = colorCan;
            dashIcon.color = colorCan;
        }
        else
        {
            timerText.color = colorCant;
            dashIcon.color = colorCant;
        }
        SetTime();
    }
}
