using UnityEngine;
using System.Collections;

/// <summary>
/// klasa SortParticleSystem
/// </summary>

public class SortParticleSystem : MonoBehaviour 
{
	/// <summary>
	/// Nazwa warstwy
	/// </summary>
    public string LayerName = "Particles";
	/// <summary>
    /// Domyślnie ParticleSystem nie ma opcji ustawiania warstwy, 
    /// na której ma być wyświetlany.
	/// </summary>
    public void Start()
    {
        particleSystem.renderer.sortingLayerName = LayerName;
    }
}
