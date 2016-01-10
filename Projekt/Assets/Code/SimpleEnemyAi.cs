using UnityEngine;

/// <summary>
/// Klasa odpowiadaj¹ca za podstawowe dzia³ania przeciwnika w œwiecie gry.
/// </summary>
public class SimpleEnemyAi : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
	/// <summary>
    /// Prêdkoœæ przeciwnika.
	/// </summary>
    public float Speed;
    /// <summary>
    /// Czêstotliwoœæ oddawania strza³ów przez przeciwnika.
    /// </summary>
	public float FireRate = 1;
    /// <summary>
    /// Pocisk.
    /// </summary>
	public Projectile Projectile;
    /// <summary>
    /// Efekt zniszczenia przeciwnika.
    /// </summary>
	public GameObject DestroyedEffect;
    /// <summary>
    /// Punkty przyznawane graczowi za zestrzelenie przeciwnika.
    /// </summary>
	public int PointsToGivePlayer;
    /// <summary>
    /// DŸwiêk strza³u przeciwnika.
    /// </summary>
    public AudioClip ShootSound;

	/// <summary>
    /// Kontroler ruchów przeciwnika, zapo¿yczony z kontrolera gracza.
	/// </summary>
    private CharacterController2D _controller;
    /// <summary>
    /// Kierunek ruchu.
    /// </summary>
	private Vector2 _direction;
    /// <summary>
    /// Pocz¹tkowa pozycja.
    /// </summary>
	private Vector2 _startPosition;
    /// <summary>
    /// Czas do oddania kolejnego strza³u.
    /// </summary>
	private float _canFireIn;

    /// <summary>
    /// Ustawienie parametrów pocz¹tkowych: kontrolera, kierunku i
    /// pozycji pocz¹tkowej.
    /// </summary>
	public void Start()
	{
		_controller = GetComponent<CharacterController2D> ();
        /// Domyœlnie porusza siê w lewo.
		_direction = new Vector2 (-1, 0);
		_startPosition = transform.position;
	}

    /// <summary>
    /// Aktualizacja po³o¿enia przeciwnika oraz obs³uga
    /// strzelania pociskami.
    /// </summary>
	public void Update()
	{
        /// Wykonanie ruchu przez przeciwnika.
        _controller.SetHorizontalForce (_direction.x * Speed);

        /// Po napotkaniu przeszkody, przeciwnik porusza siê w odwrotnym kierunku.
		if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
		{
			_direction = -_direction;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
        /// Sprawdzenie czy up³yn¹³ czas potrzebny do wystrzelenia nowrgo pocisku.
		if ((_canFireIn -= Time.deltaTime) > 0)
						return;

        /// Sprawdzenie czy gracz mo¿e zostaæ trafiony przez pocisk (jest w zasiêgu strza³u przeciwnika).
		var raycast = Physics2D.Raycast (transform.position, _direction, 10, 1 << LayerMask.NameToLayer ("Player"));
		if (!raycast)
						return;
        /// Stworzenie zmiennej pocisku.
		var projectile = (Projectile)Instantiate(Projectile, transform.position, transform.rotation);
        /// Inicjalizacja pocisku z ustawion¹ prêdkoœci¹ i kierunkiem.
		projectile.Initialize (gameObject, _direction, _controller.Velocity);
        /// Ustawienie czêstotliowœci oddawania strza³ów.
		_canFireIn = FireRate;

        /// Uruchomienie dŸwiêku wystrza³u pocisku przez przeciwnika.
        if(ShootSound != null)
            AudioSource.PlayClipAtPoint(ShootSound, transform.position);
	}

    /// <summary>
    /// Metoda wywo³ywana po otrzymaniu obra¿eñ od gracza.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
	public void TakeDamage(int damage, GameObject instigator)
	{
        /// Przyznanie punktów graczowi za zestrzelenie wroga.
        if (PointsToGivePlayer != 0) {
			var projectile = instigator.GetComponent<Projectile>();
			if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
			{
				GameManager.Instance.AddPoints(PointsToGivePlayer);
				FloatingText.Show (string.Format("+{0}", PointsToGivePlayer),"PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position,1.5f,50));
			}
		}
        /// Uruchomeinie efektu zestrzelenia przeciwnika.
		Instantiate(DestroyedEffect, transform.position, transform.rotation);
        /// Ustawienie obiektu przeciwnika jako niekatywnego.
		gameObject.SetActive (false);
	}

    /// <summary>
    /// Zrespawnowanie przeciwnika w danym checkpoincie razem z graczem.
    /// </summary>
    /// <param name="checkpoint"></param>
    /// <param name="player"></param>
	public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
	{
        /// Ustawienie jego kierunku.
        _direction = new Vector2 (-1, 0);
        /// Ustawienie jego skali w œwiecie gry jako domyœlnej.
		transform.localScale = new Vector3 (1, 1, 1);
        /// Ustawienie pozycji pocz¹tkowej.
		transform.position = _startPosition;
        /// Aktywowanie obiektu przeciwnika w œwiecie gry.
		gameObject.SetActive (true);
	}
}


