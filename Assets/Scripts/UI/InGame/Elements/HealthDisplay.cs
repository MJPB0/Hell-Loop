using UnityEngine;
using UnityEngine.UI;
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image healthContainer;
    [SerializeField] private Image healthBar;

    private Player player;
    public void SetHealth()
    {
        slider.value = player.Health;
    }
    public void SetMaxHealth()
    {
        slider.maxValue = player.MaxHealth;
    }
    void Start()
    {
        player = FindObjectOfType<Player>();

        player.OnPlayerTakeDamage += SetHealth;
        player.OnPlayerMaxHealthChange += SetMaxHealth;
        player.OnPlayerHeal += SetHealth;

        SetMaxHealth();
        SetHealth();
    }
}
