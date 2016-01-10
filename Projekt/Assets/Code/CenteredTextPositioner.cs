

using UnityEngine;

/// <summary>
/// Klasa obsługująca wyświetlanie tekstu na środku ekranu (po wejściu w nowy checkpoint).
/// </summary>
public class CenteredTextPositioner : IFloatingTextPositioner
{
    private readonly float _speed;
    private float _textPosition;

    /// <summary>
    /// Ustawienie szybkości wyświetlania komunikatów tekstowych.
    /// </summary>
    /// <param name="speed"></param>
    public CenteredTextPositioner(float speed)
    {

        _speed = speed;
    }

    /// <summary>
    /// Metoda mówiąca FloatingText, czy dalej wyświetlać tekst. 
    /// Ustawia również współrzędne tekstu w grze.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="content"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
    {

        _textPosition += Time.deltaTime * _speed;
        /// Jeśli _textPosition osiągnie 1, tekst nie będzie dalej wyświetlany.
        if (_textPosition > 1) return false;

        /// Ustawienie pozycji tekstu na ekranie.
        position = new Vector2(Screen.width / 2f - size.x / 2f, Mathf.Lerp(Screen.height / 2f + size.y, 0, _textPosition));
        return true;
    }
}


