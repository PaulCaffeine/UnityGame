
using UnityEngine;


class FloatingText : MonoBehaviour
{

    private static readonly GUISkin Skin = Resources.Load<GUISkin>("GameSkin"); ///ładujemy opcje dla textu z unity 
    public static FloatingText Show(string text, string style, IFloatingTextPositioner positioner)
    {
        var go = new GameObject("Floating Text"); ///tworzymy nowy obiekt gry
        var floatingText = go.AddComponent<FloatingText>();
        floatingText.Style = Skin.GetStyle(style); /// styl naszego tekstu
        floatingText._positioner = positioner;   /// miejsce w ktorym tekst sie pokaze
        floatingText._content = new GUIContent(text);  /// tresc ktora zaostanie pozakazana
        return floatingText;

    }
    private GUIContent _content; ///tworzymy zmienna zawierajaca tekst
    private IFloatingTextPositioner _positioner; /// tworzymy zmienna odpowiedzialna za polozenie napisu

    public string Text { get { return _content.text; } set { _content.text = value; } } /// tworzymy wlasciwosc tekst
    public GUIStyle Style { get; set; } /// tworzymy wlasciwosc styl

    public void OnGUI()
    {
        var position = new Vector2();
        var contentSize = Style.CalcSize(_content); ///obliczamy wielkosc tekstu
        if (!_positioner.GetPosition(ref position, _content, contentSize)) ///pobieramy zmienna gdzie ma byc pokazany tekst jesli jest wartosc false niszczymy objekt(tekst)
        {

            Destroy(gameObject);///usuniecie tekstu
            return;
        }

        GUI.Label(new Rect(position.x, position.y, contentSize.x, contentSize.y), _content, Style); ///metoda tworzaca tekst na ekranie
    }
}

