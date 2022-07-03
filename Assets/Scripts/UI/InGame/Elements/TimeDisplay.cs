using UnityEngine;
using TMPro;
public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeTxt;

    void Update()
    {
        FormatTime(GameplayManager.Instance.CurrentTime);
    }

    void FormatTime(float currentTime) 
    {
        int intTime = (int)currentTime;
        int sec = intTime % 60;
        int min = intTime / 60;

        string minutes = min > 9 ? min.ToString() : $"0{min}";
        string seconds = sec > 9 ? sec.ToString() : $"0{sec}";
        timeTxt.text = $"{minutes}:{seconds}";
    }
}
