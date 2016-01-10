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
    /// Dźwięk odtwarzany po zebraniu gwiazdki.
    /// </summary>
    public AudioClip HitStarSound;

    /// <summary>
    /// Obiekt animacji potrzebny do ustawienia efektu przejścia.
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// Zmienna mówiąca, czy gwiazdka została zebrana.
    /// </summary>
    private bool _isCollected;

    /// <summary>
    /// Tekstura gwiazdki.
    /// </summary>
    public SpriteRenderer Renderer;

    /// <summary>
    /// Po wejściu na gwiazdkę inicjowany jest efekt, a sam obiekt znika.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        /// Wyjście, jeśli gwiazdka została już zebrana.
        if (_isCollected)
            return;
        
        if (other.GetComponent<Player>() == null)
            return;

        /// Odtworzenie efektu dźwiękowego.
        if (HitStarSound != null)
            AudioSource.PlayClipAtPoint(HitStarSound, transform.position);

        /// Dodanie punktów.
        GameManager.Instance.AddPoints(PointsToAdd);
        /// Inicjowanie efektu graficznego.
        Instantiate(Effect, transform.position, transform.rotation);

        /// Wyświetlenie komunikatu tekstowego.
        FloatingText.Show(string.Format("+{0}!", PointsToAdd), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50)); /// metoda wyswietli tekst przez 1,5s, bedzie on sie poruszal z predkoscia 50 pixeli na sekunde

        /// Ustawienie gwiazdki jako zebranej.
        _isCollected = true;

        Animator.SetTrigger("Collect");
    }

    /// <summary>
    /// Po skończeniu animacji, tekstura jest nieaktywna.
    /// </summary>
    public void FinishAnimationEvent()
    {
        Renderer.enabled = false;
        Animator.SetTrigger("Reset");
    }

    /// <summary>
    /// Po respawnie gracza, odtwarzane są gwiazdki 
    /// zdobyte od czasu osiągnięcia ostatniego checkpointu.
    /// </summary>
    /// <param name="checkpoint"></param>
    /// <param name="player"></param>
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        _isCollected = false;
        Renderer.enabled = true;
    }
}