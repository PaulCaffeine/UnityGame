using UnityEngine;
using System.Collections;

/// <summary>
/// klasa JumpPlatform
/// </summary>

public class JumpPlatform : MonoBehaviour 
{
    /// Ustalenie sily skoku.
    public float JumpMagnitude = 20;
	/// Ustalenie JumpMagnitude
    public void ControllerEnter2D(CharacterController2D controller)
    {
        controller.SetVerticalForce(JumpMagnitude);
    }
}
