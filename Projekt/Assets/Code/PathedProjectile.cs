using UnityEngine;

public class PathedProjectile : MonoBehaviour,ITakeDamage
{
	/// <summary>
    /// Cel pocisku.
	/// </summary>
    private Transform _destination;
    /// <summary>
    /// Prêdkoœc pocisku.
    /// </summary>
	private float _speed;

    /// <summary>
    /// Efekt towarzysz¹cy zniszczeniu pocisku.
    /// </summary>
	public GameObject DestroyEffect;
    /// <summary>
    /// Punkty przyznawane graczowi za zestrzelenie pocisku.
    /// </summary>
	public int PointsToGivePlayer;
    /// <summary>
    /// DŸwiêk odtwarzany przy zniszczeniu pocisku.
    /// </summary>
    public AudioClip DestroySound;

    /// <summary>
    /// Inicjalizacja celu i prêdkoœci obiektu pocisku.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
	public void Initialize(Transform destination, float speed)
	{
		_destination = destination;
		_speed = speed;
	}

    /// <summary>
    /// Aktualizacja po³o¿enia pocisku w czasie, oraz obs³uga jego zniszczenia.
    /// </summary>
	public void Update(){
        /// Aktualizacja po³o¿enia pocisku w czasie.
		transform.position = Vector3.MoveTowards (transform.position, _destination.position, Time.deltaTime * _speed);
        /// Sprawdzenie czy pocisk dotar³ do celu.
		var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
		if (distanceSquared > .01f * .01f)
						return;
        /// Uruchomienie efektu zniszczenia pocisku.
		if (DestroyEffect != null)
						Instantiate (DestroyEffect, transform.position, transform.rotation);
        /// Odtworzenie dŸwiêku zniszczenia pocisku.
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);
        /// Zniszczenie obiektu pocisku, jeœli dotar³ do celu.
		Destroy(gameObject);
	}

    /// <summary>
    /// Metoda wywo³ywana po zestrzeleniu pocisku przez gracza.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
	public void TakeDamage(int damage, GameObject instigator)
	{
        /// Uruchomienie efektu zniszczenia pocisku.
        if (DestroyEffect != null)
						Instantiate(DestroyEffect, transform.position, transform.rotation);
        /// Zniszczenie obiektu pocisku.
		Destroy (gameObject);

		var projectile = instigator.GetComponent<Projectile>();
        /// Przyznanie punktów graczowi za zestrzelenie pocisku.
		if (projectile != null && projectile.Owner.GetComponent<Player> () != null && PointsToGivePlayer != 0) {
			GameManager.Instance.AddPoints(PointsToGivePlayer);
			FloatingText.Show (string.Format ("+{0}!",PointsToGivePlayer),"PointStarText", new FromWorldPointTextPositioner(Camera.main,transform.position,1.5f,50));

		}
	
	}
}