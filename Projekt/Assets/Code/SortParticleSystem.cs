using UnityEngine;
using System.Collections;

public class SortParticleSystem : MonoBehaviour 
{
    public string LayerName = "Particles";

    // Domyślnie ParticleSystem nie ma opcji ustawiania warstwy, 
    // na której ma być wyświetlany.
    public void Start()
    {
        particleSystem.renderer.sortingLayerName = LayerName;
    }
}
