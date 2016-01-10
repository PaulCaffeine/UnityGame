

using UnityEngine;

/// <summary>
/// Klasa określająca parametry wyświetlanego tekstu w świecie gry.
/// </summary>
class FromWorldPointTextPositioner : IFloatingTextPositioner
{
    /// <summary>
    /// Zmienna ktora pomoze nam okreslic gdzie na ekranie poszczegolny obiekt powienien byc relatywnie do swoich współrzędnych świata.
    /// </summary>
    private readonly Camera _camera;
    /// <summary>
    /// Zmienna okreslajaca pozycję tekstu.
    /// </summary>
    private readonly Vector3 _worldPosition;
    /// <summary>
    /// Zmienna odpowiadająca za czas pokazywania się tekstu na ekranie.
    /// </summary>
    private float _timeToLive; 
    /// <summary>
    /// Prędkość poruszania się tekstu w górę.
    /// </summary>
    private readonly float _speed; 
    /// <summary>
    /// Offset określający zmianę położenia podczas ruchu napisu.
    /// </summary>
    private float _yOffset;

    /// <summary>
    /// Konstruktor pozycjonera tekstu w grze, przyjmujący jako parametry pozycję tekstu w świecie gry,
    /// obiekt kamery, czas, przez jaki ma być wyświetlany tekst i prędkość poruszania się tekstu.
    /// </summary>
    /// <param name="Camera"></param>
    /// <param name="worldPosition"></param>
    /// <param name="timeToLive"></param>
    /// <param name="speed"></param>
    public FromWorldPointTextPositioner(Camera Camera, Vector3 worldPosition, float timeToLive, float speed)
    {
        _camera = Camera;
        _worldPosition = worldPosition;
        _timeToLive = timeToLive;
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
        /// Jeśli podczas wyświetlania tekstu (dana klatka), upłynie czas życia napisu, metoda zwraca false.
        /// Klasa FloatingText niszczy wtedy tekst.
        if ((_timeToLive -= Time.deltaTime) <= 0) return false;

        var screenPosition = _camera.WorldToScreenPoint(_worldPosition);
        /// Środkujemy tekst na współrzędnych świata.
        position.x = screenPosition.x - (size.x / 2);
        position.y = Screen.height - screenPosition.y - _yOffset;

        _yOffset += Time.deltaTime * _speed;
        return true;
    }
}


