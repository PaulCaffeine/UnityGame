
using UnityEngine;
/// <summary>
/// Interfejs z metodą pozwalającą pobrać położenie tekstu wyświetlanego na ekranie.
/// </summary>
interface IFloatingTextPositioner
{
    bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size); ///metoda pozwalajaca pobrac polozenie tekstu

}


