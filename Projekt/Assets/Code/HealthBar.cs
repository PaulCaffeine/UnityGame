using UnityEngine;

/// <summary>
/// Klasa pasku zdrowia pojawiającego się nad graczem.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Player Player;
    public Transform ForegroundSprite;
    public SpriteRenderer ForegroundRenderer;
    /// <summary>
    /// Ustawienie koloru maksymalnej wartości zdrowia na niebieski.
    /// </summary>
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    /// <summary>
    /// Ustawienie koloru minimalnej wartości zdrowia na czerwony.
    /// </summary>
    public Color MinHealthColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);

    /// <summary>
    /// Aktualizacja punktów zdrowia gracza oraz odpowiadającego koloru.
    /// </summary>
    public void Update()
    {
        var healthPercent = Player.Health / (float)Player.MaxHealth;

        ForegroundSprite.localScale = new Vector3(healthPercent, 1, 1);
        ForegroundRenderer.color = Color.Lerp(MaxHealthColor, MinHealthColor, healthPercent);
    }
}

