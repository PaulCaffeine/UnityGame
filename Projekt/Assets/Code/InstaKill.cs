using UnityEngine;

/// <summary>
/// Klasa odpowiedzialna za zabicie gracza, gdy napotka na Collider2D danego obiektu.
/// </summary>
public class InstaKill : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        LevelManager.Instance.KillPlayer();
    }
}


