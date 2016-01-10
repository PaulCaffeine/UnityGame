using UnityEngine;

/// <summary>
/// Tworzenie pocisku poruszającego się po danej ścieżce.
/// </summary>
public class PathedProjectileSpawner : MonoBehaviour {

    /// <summary>
    /// Cel.
    /// </summary>
	public Transform Destination;
    /// <summary>
    /// Obiekt pocisku.
    /// </summary>
	public PathedProjectile Projectile;
		

	public GameObject SpawnEffect;
    /// <summary>
    /// Prędkość pocisku.
    /// </summary>
	public float Speed;
    /// <summary>
    /// Częstotliwość oddawania strzałów.
    /// </summary>
	public float FireRate;
    /// <summary>
    /// Dźwięk odtwarzany podczas wystrzału pocisku.
    /// </summary>
    public AudioClip SpawnProjectileSound;
    /// <summary>
    /// Obiekt animacji potrzebny do ustawienia efektu przejścia.
    /// </summary>
    public Animator Animator;
    /// <summary>
    /// Zmienna służąca do odmierzania czasu do kolejnego wystrzału.
    /// </summary>
	private float _nextShotInSeconds;

    /// <summary>
    /// Ustawienie początkowej wartości czasu kolejnego wystrzału na częstotliwość oddawania strzałów.
    /// </summary>
	public void Start()
	{
		_nextShotInSeconds = FireRate;
	}

    /// <summary>
    /// Metoda odmierzająca czas do kolejnego wystrzału, inicjująca obiekt pocisku 
    /// w danym położeniu, o danej prędkości i wyznaczonym celu. Odpowiada również za
    /// inicjowanie efektu stworzenia pocisku w świecie gry oraz odtworzenie dźwięku.
    /// </summary>
	public void Update()
	{
		if ((_nextShotInSeconds -= Time.deltaTime) > 0)
						return;
		_nextShotInSeconds = FireRate;
		var projectile = (PathedProjectile)Instantiate (Projectile, transform.position, transform.rotation);
		projectile.Initialize (Destination, Speed);

		if (SpawnEffect != null)
						Instantiate(SpawnEffect, transform.position, transform.rotation);

        if (SpawnProjectileSound != null)
            AudioSource.PlayClipAtPoint(SpawnProjectileSound, transform.position);

        if (Animator != null)
            Animator.SetTrigger("Fire");
	}

    /// <summary>
    /// Tworzenie czerwonej linii w widoku Scene podczas wystrzału,
    /// od jego źródła do celu.
    /// </summary>
	public void OnDrawGizmos()
	{
		if (Destination == null)
						return;

		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, Destination.position);
	}
}
