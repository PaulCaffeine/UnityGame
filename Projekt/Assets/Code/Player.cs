﻿using UnityEngine;
using System.Collections;
/// <summary>
/// klasa Player
/// </summary>
public class Player : MonoBehaviour, ITakeDamage
{
	/// <summary>
    /// Czy gracz jest zwrocony w prawo.
	/// </summary>
    private bool _isFacingRight;
    private CharacterController2D _controller;
	/// <summary>
    /// Wartosc rowna 1 lub -1, zalezna od kierunku w jakim
    /// porusza sie gracz.
	/// </summary>
    private float _normalizedHorizontalSpeed;
	/// <summary>
    /// Maksymalna liczba jednostek na sekunde, jakie gracz moze przejsc.
	/// </summary>
    public float MaxSpeed = 8;
	/// <summary>
    /// Jak szybko moze zmienic sie predkosc gracza na ziemi.
	/// </summary>
    public float SpeedAccelerationOnGround = 10f;
	/// <summary>
    /// Jak szybko moze zmienic sie predkosc gracza w powietrzu
	/// </summary>
    public float SpeedAccelerationInAir = 5f;

    /// <summary>
    /// Maksymalna wartość punktów zdrowia.
    /// </summary>
    public int MaxHealth = 100;
    /// <summary>
    /// Efekt wyzwalany podczas otrzymania obrażeń.
    /// </summary>
    public GameObject OuchEffect;
	/// <summary>
	/// Piła
	/// </summary>
	public Projectile Projectile;
	/// <summary>
	/// Częstotliwość z którą strzela
	/// </summary>
	public float FireRate;
	/// <summary>
	/// Lokalizacja Piły.
	/// </summary>
	public Transform ProjectileFireLocation;
	/// <summary>
	/// Fire projectile effect.
	/// </summary>
	public GameObject FireProjectileEffect;

    /// <summary>
    /// Dźwięk odtwarzany po trafieniu gracza przez wroga.
    /// </summary>
    public AudioClip PlayerHitSound;
    /// <summary>
    /// Dźwięk odtwarzany po wystrzeleniu pocisku przez gracza.
    /// </summary>
    public AudioClip PlayerShootSound;
    /// <summary>
    /// Dźwięk odtwarzany po zebraniu apteczki przez gracza.
    /// </summary>
    public AudioClip PlayerHealthSound;

    public Animator Animator;

    /// <summary>
    /// Wartośc punktów zdrowia.
    /// </summary>
    public int Health { get; private set; }
    /// <summary>
    /// Zmienna bool służąca do zapamiętania informacji o śmierci gracza.
    /// </summary>
    public bool IsDead { get; private set; }
	/// <summary>
	/// Jeżeli gracz może strzelać
	/// </summary>
	private float _canFireIn;

    /// <summary>
    /// Ustawianie początkowych ustawień gracza.
    /// </summary>
    public void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        /// Jesli gracz jest obrocony, wartosc localScale.x bedzie mniejsza od 0.
        _isFacingRight = transform.localScale.x > 0;
        /// Punkty zdrowia mają wartość maksymalną.
        Health = MaxHealth;
    }
	/// <summary>
    /// Wywolywane dla kazdej klatki animacji gry.
    /// Wywolanie metody HandleImput(), obslugujacej nacisniecie klawiszy przez
    /// gracza. Ustawiana jest predkosc w poziomie, zalezna od
    /// kierunku zwrotu gracza, oraz tego czy znajduje sie w powietrzu lub na ziemi.
	/// </summary>
    public void Update()
    {
        /// Aktualizacja czasu przeładowywania broni.
        _canFireIn -= Time.deltaTime;
        if (!IsDead)
            HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        if (IsDead)
            _controller.SetHorizontalForce(0);
        else
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        Animator.SetBool("IsGrounded", _controller.State.IsGrounded);
        Animator.SetBool("IsDead", IsDead);
        /// Ustawienie wartości prędkości jako liczby między 0, a 1.
        Animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x) / MaxSpeed);
    }

    /// <summary>
    /// Metoda uruchamiana po przejściu danego poziomu gry.
    /// Deaktywuje ona BoxCollider2D i ruch gracza.
    /// </summary>
    public void FinishLevel()
    {
        enabled = false;
        _controller.enabled = false;
        collider2D.enabled = false;
    }

    /// <summary>
    /// Zresetowanie ustawień gracza po śmierci.
    /// </summary>
    public void Kill()
    {
        _controller.HandleCollisions = false;
        collider2D.enabled = false;
        IsDead = true;
        Health = 0;

        /// Gracz lekko podskakuje pionowo po śmierci.
        _controller.SetForce(new Vector2(0, 20));
    }

    /// <summary>
    /// Respawnowanie gracza w danym punkcie i zmiana jego ustawień początkowych.
    /// </summary>
    /// <param name="spawnPoint"></param>
    public void RespawnAt(Transform spawnPoint)
    {
        if (!_isFacingRight)
            Flip();

        IsDead = false;
        collider2D.enabled = true;
        _controller.HandleCollisions = true;
        Health = MaxHealth;

        transform.position = spawnPoint.position;
    }

    /// <summary>
    /// Inicjowanie efektu obrażeń gracza (czerwona chmura cząsteczek) 
    /// oraz zmniejszenie punktów zdrowia gracza po zadanych obrażeniach,
    /// prowadzące ewentualnie do jego śmierci.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage, GameObject instagator)
    {
        AudioSource.PlayClipAtPoint(PlayerHitSound, transform.position);
        FloatingText.Show(string.Format("-{0}", damage), "PlayerTakeDamageText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60f));

        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;

        if (Health <= 0)
            LevelManager.Instance.KillPlayer();
    }

    /// <summary>
    /// Dodanie punktów zdrowia po zebraniu apteczki.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="instagator"></param>
    public void GiveHealth(int health, GameObject instagator)
    {
        AudioSource.PlayClipAtPoint(PlayerHealthSound, transform.position);
        FloatingText.Show(string.Format("+{0}!", health), "PlayerGotHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60f));
		///Health += Mathf.Min(Health + health, MaxHealth);
        Health = Mathf.Min(Health + health, MaxHealth);
    }

	/// <summary>
    /// Obsluga interakcji gracza (nacisniecia klawisza A lub D), umozliwiajaca obrot.
    /// Nacisniecie spacji wykonuje skok.
	/// </summary>
    public void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
            if (_isFacingRight)
                Flip();
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }

        if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();
        }
		if (Input.GetMouseButtonDown(0))
						FireProjectile ();
    }

    /// <summary>
    /// Metoda odpowiadająca za wystrzelenie pocisku przez gracza.
    /// </summary>
	private void FireProjectile()
	{
        /// Sprawdzenie, czy upłynął czas ładowania broni.
        if (_canFireIn > 0)
						return;

        /// Ustawienie obiektu efektu wystrzału i umiejscowanie go w grze.
		if (FireProjectileEffect != null) {
						var effect = (GameObject) Instantiate(FireProjectileEffect, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
						effect.transform.parent = transform;
				}

        /// Ustalenie kierunku strzału na podstawie 
        /// aktualnego zwrotu postaci gracza.
		var direction = _isFacingRight ? Vector2.right : -Vector2.right;

        /// Oddanie strzału.
		var projectile = (Projectile) Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
		projectile.Initialize(gameObject, direction, _controller.Velocity);

        /// Ustawienie, zmniejszanej w metodzie Update, wartości częstotliwości oddawania strzałów przez gracza.
		_canFireIn = FireRate;

        AudioSource.PlayClipAtPoint(PlayerShootSound, transform.position);

        Animator.SetTrigger("Fire");
	}
	/// <summary>
    /// Obrocenie gracza w poziomie.
	/// </summary>
    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
    }
}
