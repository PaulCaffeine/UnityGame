
using UnityEngine;

/// <summary>
/// Klasa odpowiedzialna za wyświetlanie na ekranie gry tekstu
/// informującego o różnych zdarzeniach występujących w czasie rozgrywki.
/// </summary>
class FloatingText : MonoBehaviour
{
    /// <summary>
    /// Załadowanie GUISkin dla interfejsu gry w Unity.
    /// </summary>
    private static readonly GUISkin Skin = Resources.Load<GUISkin>("GameSkin"); 
    public static FloatingText Show(string text, string style, IFloatingTextPositioner positioner)
    {
        /// Stworzenie nowego obiektu gry.
        var go = new GameObject("Floating Text"); 
        var floatingText = go.AddComponent<FloatingText>();
        /// Ustawienie stylu tekstu.
        floatingText.Style = Skin.GetStyle(style);
        /// Ustawienie miejsca, w którym wyświetlony zostanie tekst.
        floatingText._positioner = positioner;
        /// Ustawienie treści tekstu, która zostanie pokazany.
        floatingText._content = new GUIContent(text);  
        return floatingText;

    }
    /// <summary>
    /// Pole interfejsu użytkownika zawierające tekst.
    /// </summary>
    private GUIContent _content; 
    /// <summary>
    /// Zmienna odpowiedzialna za położenie napisu.
    /// </summary>
    private IFloatingTextPositioner _positioner;

    /// Wyświetlany tekst.
    public string Text { get { return _content.text; } set { _content.text = value; } }
    /// Zmienna stylu interfejsu użytkownika.
    public GUIStyle Style { get; set; } 

    /// <summary>
    /// Metoda pokazująca tekst na ekranie oraz usuwająca go.
    /// </summary>
    public void OnGUI()
    {
        var position = new Vector2();
        /// Obliczenie wielkości tekstu.
        var contentSize = Style.CalcSize(_content);
        /// Jeśli wartość zmiennej wskazującej, gdzie ma być wyświetlony 
        /// tekst jest pusta, niszczony jest obiekt tekstu.
        if (!_positioner.GetPosition(ref position, _content, contentSize))
        {
            /// Usunięcie tekstu.
            Destroy(gameObject);
            return;
        }
        /// Metoda tworząca tekst na ekranie.
        GUI.Label(new Rect(position.x, position.y, contentSize.x, contentSize.y), _content, Style); 
    }
}

