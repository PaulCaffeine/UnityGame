using UnityEngine;

/// <summary>
/// Klasa potomna dziedzicz¹ca z Projectile.
/// </summary>
public class SimpleProjectile : Projectile
{
	/// <summary>
    /// Obra¿enia zadane przez pocisk.
	/// </summary>
    public int Damage;
    /// <summary>
    /// Efekt zniszczenia pocisku.
    /// </summary>
	public GameObject DestroyedEffect;
    /// <summary>
    /// Punkty przyznawane graczowi, jeœli uda mu siê zniszczyæ (w locie) pocisk przeciwnika.
    /// </summary>
	public int PointsToGiveToPlayer;
    /// <summary>
    /// Czas ¿ycia pocisku.
    /// </summary>
	public float TimeToLive;
    /// <summary>
    /// DŸwiêk odtwarzany przy zniszczeniu pocisku
    /// </summary>
    public AudioClip DestroySound;

    /// <summary>
    /// Metoda aktualizuj¹ca pozycjê pocisku w Unity, oraz odpowiadaj¹ca za 
    /// jego zniszczenie po up³ywie czasu ¿ycia pocisku. 
    /// </summary>
	public void Update()
	{
		if ((TimeToLive - + Time.deltaTime) <= 0) {
			DestroyProjectile();
			return;
		}

		transform.Translate (Direction * ((Mathf.Abs (InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);
	}

    /// <summary>
    /// Metoda odpowiadaj¹ca za zniszczenie pocisku i przyznanie punktów graczowi,
    /// jesli uda mu siê zestrzeliæ pocisk przeciwnika.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
	public void TakeDamage(int damage, GameObject instigator)
	{
		if (PointsToGiveToPlayer != 0) {
		
			var projectile = instigator.GetComponent<Projectile>();
			if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
			{
				GameManager.Instance.AddPoints(PointsToGiveToPlayer);
				FloatingText.Show(string.Format("+{0}!",PointsToGiveToPlayer), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f,50));
			}
		}
		DestroyProjectile();
	}

    /// <summary>
    /// Podczas kolizji z otoczeniem nastêpi zniszczenie pocisku.
    /// </summary>
    /// <param name="other"></param>
	protected override void OnCollideOther (Collider2D other) {
		DestroyProjectile();
	}

    /// <summary>
    /// Metoda wywo³ywana po trafieniu przez pocisk obiektu,
    /// który mo¿e przyj¹æ obra¿enia.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="takeDamage"></param>
	protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
	{
		takeDamage.TakeDamage(Damage, gameObject);
		DestroyProjectile();
	}

    /// <summary>
    /// Metoda odpowiadaj¹ca za zniszczenie pocisku.
    /// </summary>
	private void DestroyProjectile()
	{
        /// Uruchomienie efektu zniszczenia pocisku.
        if (DestroyedEffect != null)
						Instantiate(DestroyedEffect, transform.position, transform.rotation);
        /// Odtworzenie dŸwiêku zniszczenia pocisku.
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);
        /// Zniszczenie obiektu pocisku.
		Destroy(gameObject);
	}
}
	

