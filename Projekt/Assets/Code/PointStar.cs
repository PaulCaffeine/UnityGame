using UnityEngine;

/// <summary>
/// Klasa przyznająca punkty za zebranie gwiazdek.
/// </summary>
public class PointStar : MonoBehaviour, IPlayerRespawnListener
{
    /// <summary>
    /// Efektem jest chmura żółtych cząsteczek.
    /// </summary>
    public GameObject Effect;
    /// <summary>
    /// Liczba punktów dodawana za zebranie gwiazdki.
    /// </summary>
    public int PointsToAdd = 10;

    /// <summary>
    /// Po wejściu na gwiazdkę inicjowany jest efekt, a sam obiekt znika.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        GameManager.Instance.AddPoints(PointsToAdd);
        Instantiate(Effect, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Po respawnie gracza, odtwarzane są gwiazdki 
    /// zdobyte od czasu osiągnięcia ostatniego checkpointu.
    /// </summary>
    /// <param name="checkpoint"></param>
    /// <param name="player"></param>
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
}