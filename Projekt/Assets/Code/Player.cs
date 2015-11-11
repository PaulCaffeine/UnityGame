using UnityEngine;
using System.Collections;
/// <summary>
/// klasa Player
/// </summary>
public class Player : MonoBehaviour 
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

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        /// Jesli gracz jest obrocony, wartosc localScale.x bedzie mniejsza od 0.
        _isFacingRight = transform.localScale.x > 0;
    }
	/// <summary>
    /// Wywolywane dla kazdej klatki animacji gry.
    /// Wywolanie metody HandleImput(), obslugujacej nacisniecie klawiszy przez
    /// gracza. Ustawiana jest predkosc w poziomie, zalezna od
    /// kierunku zwrotu gracza, oraz tego czy znajduje sie w powietrzu lub na ziemi.
	/// </summary>
    public void Update()
    {
        HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
        _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
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
