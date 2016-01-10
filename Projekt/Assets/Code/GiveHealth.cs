using UnityEngine;

/// <summary>
/// Klasa odpowiadająca za dodanie punktów zdrowia gracza po zebraniu apteczki.
/// </summary>
public class GiveHealth : MonoBehaviour, IPlayerRespawnListener
{
    /// <summary>
    /// Efekt graficzny towarzyszący zebraniu apteczki.
    /// </summary>
    public GameObject Effect;
    /// <summary>
    /// Ilośc punktów zdrowia dodawanych przez zebranie apteczki.
    /// </summary>
    public int HealthToGive;

    /// <summary>
    /// Metoda uruchamiana po zebraniu apteczki.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        /// Dodanie punktów zdrowia.
        player.GiveHealth(HealthToGive, gameObject);
        /// Zaincijowanie efektu graficznego.
        Instantiate(Effect, transform.position, transform.rotation);

        /// Usunięcie apteczki po jej zebraniu ze świata gry w Unity.
        gameObject.SetActive(false);

        //FloatingText.Show(string.Format("+{0}!", HealthToGive), "GiveHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
    }

    /// <summary>
    /// Respawnowanie apteczki po powrocie do ostatniego checkpointu.
    /// </summary>
    /// <param name="checkpoint"></param>
    /// <param name="player"></param>
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
    
}

