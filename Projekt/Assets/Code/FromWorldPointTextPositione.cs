

using UnityEngine;

class FromWorldPointTextPositioner : IFloatingTextPositioner
{


    private readonly Camera _camera; ///zmienna ktora pomoze nam okreslic gdzie na ekranie poszczegolny obiekt powienien byc relatywnie do swoich współrzędnych świata
    private readonly Vector3 _worldPosition;  ///zmienna okreslajaca pozycje tekstu
    private float _timeToLive; /// zmienna odpowiadajaca za czas pokazywania sie tekstu na ekranie
    private readonly float _speed; /// predkosc poruszana sie ( tekstu)
    private float _yOffset;



    public FromWorldPointTextPositioner(Camera Camera, Vector3 worldPosition, float timeToLive, float speed) ///konstruktor przypisujacy wartosc poszczegolnym zmiennym
    {
        _camera = Camera;
        _worldPosition = worldPosition;
        _timeToLive = timeToLive;
        _speed = speed;



    }
    public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
    {
        if ((_timeToLive -= Time.deltaTime) <= 0) return false;

        var screenPosition = _camera.WorldToScreenPoint(_worldPosition);
        position.x = screenPosition.x - (size.x / 2); /// środkujemy tekst na  współrzędnych świata
        position.y = Screen.height - screenPosition.y - _yOffset;

        _yOffset += Time.deltaTime * _speed;
        return true;


    }
}


