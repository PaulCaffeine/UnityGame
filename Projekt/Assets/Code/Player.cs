using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    // Czy gracz jest zwrocony w prawo.
    private bool _isFacingRight;
    private CharacterController2D _controller;
    // Wartosc rowna 1 lub -1, zalezna od kierunku w jakim
    // porusza sie gracz.
    private float _normalizedHorizontalSpeed;

    // Maksymalna liczba jednostek na sekunde, jakie gracz moze przejsc.
    public float MaxSpeed = 8;
    // Jak szybko moze zmienic sie predkosc gracza w powietrzu i na ziemi.
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        // Jesli gracz jest obrocony, wartosc localScale.x bedzie mniejsza od 0.
        _isFacingRight = transform.localScale.x > 0;
    }

    // Wywolywane dla kazdej klatki animacji gry.
    // Wywolanie metody HandleImput(), obslugujacej nacisniecie klawiszy przez
    // gracza. Ustawiana jest predkosc w poziomie, zalezna od
    // kierunku zwrotu gracza, oraz tego czy znajduje sie w powietrzu lub na ziemi.
    public void Update()
    {
        HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
        _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
    }

    // Obsluga interakcji gracza (nacisniecia klawisza A lub D), umozliwiajaca obrot.
    // Nacisniecie spacji wykonuje skok.
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

    // Obrocenie gracza w poziomie.
    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
    }
}
