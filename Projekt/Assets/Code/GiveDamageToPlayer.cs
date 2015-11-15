using UnityEngine;

/// <summary>
/// Klasa odpowiedzialna za zadawanie obrażeń graczowi przez dany obiekt.
/// </summary>
public class GiveDamageToPlayer : MonoBehaviour
{
    /// <summary>
    /// Domyślna wartośc obrażeń zadawana graczowi.
    /// </summary>
    public int DamageToGive = 10;

    private Vector2
        _lastPosition,
        _velocity;

    /// <summary>
    /// Aktualizacja położenia obiektu.
    /// </summary>
    public void LateUpdate()
    {
        _velocity = (_lastPosition - (Vector2)transform.position) / Time.deltaTime;
        _lastPosition = transform.position;
    }

    /// <summary>
    /// Odebranie punktów zdrowia gracza po zderzeniu z Colliderem2D obiektu,
    /// oraz ustawienie lekkiego "odrzutu" gracza w tył po otrzymaniu obrażeń.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.TakeDamage(DamageToGive);
        var controller = player.GetComponent<CharacterController2D>();
        var totalVelocity = controller.Velocity + _velocity;

        controller.SetForce(new Vector2(
            -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 6, 10, 40),
            -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 2, 0, 15)));
    }
}

