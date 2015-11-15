using UnityEngine;

/// <summary>
/// Klasa zapewnia podążanie pasku za graczem (w odpowiedniej odległości).
/// </summary>
public class FollowObject : MonoBehaviour
{
    public Vector2 Offset;
    public Transform Following;

    /// <summary>
    /// Aktualizacja położenia pasku zdrowia na podstawie ruchu gracza.
    /// </summary>
    public void Update()
    {
        transform.position = Following.transform.position + (Vector3)Offset;
    }
}
