using UnityEngine;

/// <summary>
/// Klasa potomna dziedzicz�ca z Projectile.
/// </summary>
public class SimpleProjectile : Projectile
{
	/// <summary>
    /// Obra�enia zadane przez pocisk.
	/// </summary>
    public int Damage;
    /// <summary>
    /// Efekt zniszczenia pocisku.
    /// </summary>
	public GameObject DestroyedEffect;
    /// <summary>
    /// Punkty przyznawane graczowi, je�li uda mu si� zniszczy� (w locie) pocisk przeciwnika.
    /// </summary>
	public int PointsToGiveToPlayer;
    /// <summary>
    /// Czas �ycia pocisku.
    /// </summary>
	public float TimeToLive;
    /// <summary>
    /// D�wi�k odtwarzany przy zniszczeniu pocisku
    /// </summary>
    public AudioClip DestroySound;

    /// <summary>
    /// Metoda aktualizuj�ca pozycj� pocisku w Unity, oraz odpowiadaj�ca za 
    /// jego zniszczenie po up�ywie czasu �ycia pocisku. 
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
    /// Metoda odpowiadaj�ca za zniszczenie pocisku i przyznanie punkt�w graczowi,
    /// jesli uda mu si� zestrzeli� pocisk przeciwnika.
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
    /// Podczas kolizji z otoczeniem nast�pi zniszczenie pocisku.
    /// </summary>
    /// <param name="other"></param>
	protected override void OnCollideOther (Collider2D other) {
		DestroyProjectile();
	}

    /// <summary>
    /// Metoda wywo�ywana po trafieniu przez pocisk obiektu,
    /// kt�ry mo�e przyj�� obra�enia.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="takeDamage"></param>
	protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
	{
		takeDamage.TakeDamage(Damage, gameObject);
		DestroyProjectile();
	}

    /// <summary>
    /// Metoda odpowiadaj�ca za zniszczenie pocisku.
    /// </summary>
	private void DestroyProjectile()
	{
        /// Uruchomienie efektu zniszczenia pocisku.
        if (DestroyedEffect != null)
						Instantiate(DestroyedEffect, transform.position, transform.rotation);
        /// Odtworzenie d�wi�ku zniszczenia pocisku.
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);
        /// Zniszczenie obiektu pocisku.
		Destroy(gameObject);
	}
}
	

