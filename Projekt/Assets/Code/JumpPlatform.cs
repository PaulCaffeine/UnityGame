using UnityEngine;
using System.Collections;

/// <summary>
/// klasa JumpPlatform
/// </summary>

public class JumpPlatform : MonoBehaviour 
{
    /// Ustalenie sily skoku.
    public float JumpMagnitude = 20;

    public AudioClip JumpSound;

	/// Ustalenie JumpMagnitude
    public void ControllerEnter2D(CharacterController2D controller)
    {
        if (JumpSound != null)
            AudioSource.PlayClipAtPoint(JumpSound, transform.position);
        
        controller.SetVerticalForce(JumpMagnitude);
    }
}
