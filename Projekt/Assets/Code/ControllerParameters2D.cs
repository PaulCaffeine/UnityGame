using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ControllerParameters2D 
{
    // Czy mozna wykonac skok i w jakich okolicznosciach.
    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }

    // Ustawienie maksymalnej predkosci gracza.
    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    // Nie mozna wejsc na teren o nachyleniu wiekszym od 30 stopni. 
    // Ustalenie grawitacji, dopusczalnej czestotliwosci skokow i ich sily.
    [Range(0,90)]
    public float SlopeLimit = 30;
    public float Gravity = -25f;

    public JumpBehavior JumpRestrictions;

    public float JumpFrequency = .25f;

    public float JumpMagnitude = 12;
}
