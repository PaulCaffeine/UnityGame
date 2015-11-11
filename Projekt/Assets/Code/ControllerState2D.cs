using UnityEngine;

using System.Collections;

/// <summary>
/// klasa ControllerState2D 
/// </summary>

public class ControllerState2D 
{
	/// <summary>
	/// Kolizja z prawej strony
	/// </summary>
    public bool IsCollidingRight { get; set; }
	/// <summary>
	/// Kolizja z lewej strony
	/// </summary>
    public bool IsCollidingLeft { get; set; }
	/// <summary>
	/// Kolizja powyżej
	/// </summary>
    public bool IsCollidingAbove { get; set; }
	/// <summary>
	/// Kolizja z poniżej
	/// </summary>
    public bool IsCollidingBelow { get; set; }
	/// <summary>
	/// Kolizja ruch po pochyłej (dół)
	/// </summary>
    public bool IsMovingDownSlope { get; set; }
	/// <summary>
	/// Kolizja ruch po pochyłej (góra)
	/// </summary>
    public bool IsMovingUpSlope { get; set; }
	/// <summary>
	/// Kolizja ziemia
	/// </summary>
    public bool IsGrounded { get { return IsCollidingBelow;  } }
	/// <summary>
	/// Kąt pochyłej
	/// </summary>
    public float SlopeAngle { get; set; }
	/// <summary>
	/// Kolizja jakakolwiek
	/// </summary>
    public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }
	/// <summary>
	/// Reset stanu
	/// </summary>
    public void Reset()
    {
        IsMovingUpSlope =
            IsMovingDownSlope =
            IsCollidingLeft =
            IsCollidingRight =
            IsCollidingAbove =
            IsCollidingBelow = false;

        SlopeAngle = 0;
    }
	/// <summary>
	/// Debug
	/// </summary>
    public override string ToString()
    {
        return string.Format("(controller: r: {0} l: {1} a: {2} b: {3} down-slope: {4} up-slope: {5} angle: {6} )", 
            IsCollidingRight, IsCollidingLeft, IsCollidingAbove, IsCollidingBelow, IsMovingDownSlope, IsMovingUpSlope, SlopeAngle);
    }
}
