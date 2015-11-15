using UnityEngine;

/// <summary>
/// Klasa automatycznie niszcząca particle system po zakończeniu jego animacji.
/// </summary>

public class AutoDestroyParticleSystem : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    /// <summary>
    /// Pobranie ParticleSystem.
    /// </summary>
    public void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Automatyczne zniszczenie particle system po zakończeniu jego animacji.
    /// </summary>
    public void Update()
    {
        if (_particleSystem.isPlaying)
            return;

        Destroy(gameObject);
    }
}
