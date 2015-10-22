using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    // Obiekt gracza.
    public Transform Player;
    // Wartość progu ruchu gracza, od którego 
    // ma nastapić zmiana położenia kamery.
    public Vector2 Margin;
    // Zapewnienie płynności ruchu kamery.
    public Vector2 Smoothing;
    // Ograniczenia ruchu kamery w świecie gry.
    public BoxCollider2D Bounds;

    private Vector3 _min, _max;

    // Zmienna, mówiąca, czy kamera podąża za graczem.
    public bool IsFollowing { get; set; }

    public void Start()
    {
        // Ustawienie ograniczeń rozmiaru obszaru,
        // w którym może się poruszać kamera,
        // oraz ustawienie podążania kamery za graczem.
        _min = Bounds.bounds.min;
        _max = Bounds.bounds.max;
        IsFollowing = true;
    }

    public void Update()
    {
        // Obecne położenie kamery.
        var x = transform.position.x;
        var y = transform.position.y;

        if (IsFollowing)
        {
            // Jeśli odległość między położeniem kamery, a położeniem gracza
            // w poziomie jest większa od danej wartości.
            if (Mathf.Abs(x - Player.position.x) > Margin.x)
            {
                // Wykonaj ruch kamery z pozycji x do pozycji gracza,
                // z określonym efektem płynności w grze.
                x = Mathf.Lerp(x, Player.position.x, Smoothing.x * Time.deltaTime);
            }

            // Jeśli odległość między położeniem kamery, a położeniem gracza
            // w pionie jest większa od danej wartości.
            if (Mathf.Abs(y - Player.position.y) > Margin.y)
            {
                // Wykonaj ruch kamery z pozycji y do pozycji gracza,
                // z określonym efektem płynności w grze.
                y = Mathf.Lerp(y, Player.position.y, Smoothing.y * Time.deltaTime);
            }
        }

        // camera.ortographicSize to połowa wysokości boxu kamery.
        // Mnożymy tę wartośc przez stosunek szerokości do wysokości, 
        // aby otrzymać połowę szerokości boxu kamery.
        var cameraHalfWidth = camera.orthographicSize * ((float) Screen.width / Screen.height);

        // Ograniczanie położenia kamery (x,y) do rozmiarów Bounds,
        // czyli boxu ograniczającego ruch kamery w świecie gry.
        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, _min.y + camera.orthographicSize, _max.y - camera.orthographicSize);

        // Ustawienie uaktualnionej pozycji kamery.
        transform.position = new Vector3(x, y, transform.position.z);
    }

}
