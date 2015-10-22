using UnityEngine;
using System.Collections;

public class BackgroundParallax : MonoBehaviour 
{
    // Tablica rzędów elementów tła.
    public Transform[] Backgrounds;
    // Wartość efektu poruszania się tła.
    public float ParallaxScale;
    // Wartość redukująca stopień poruszania się,
    // dla warstw (rzędów elementów) dalej położonych.
    public float ParallaxReductionFactor;
    // Zapewnienie płynności ruchu.
    public float Smoothing;

    private Vector3 _lastPosition;

    public void start()
    {
        // Zapamiętanie położenia kamery w ostatniej klatce gry, 
        // potrzebnego w późniejszym etape do określenia,
        // o ile tło musi się poruszyć w lewo lub prawo.
        _lastPosition = transform.position;
    }

    public void Update()
    {
        // Ruch wykonany od ostaniej klatki do obecnej, przeskalowany przez czynnik efektu
        // parallax (intensywności ruchu tła).
        var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;

        // Dla każdej warstwy (rzędu elementów) tła.
        for (var i = 0; i < Backgrounds.Length; i++)
        {
            // Zapamiętanie pozycji warstwy tła uaktualnionej o efekt parallax, 
            // oraz postepującą redukcję tego efektu dla coraz dalszych warstw.
            var backgroundTargetPosition = Backgrounds[i].position.x + parallax * (i * ParallaxReductionFactor + 1);
            // Ustawienie aktualnej pozycji warstw tła.
            Backgrounds[i].position = Vector3.Lerp(
                Backgrounds[i].position, // Obecna pozycja tła.
                new Vector3(backgroundTargetPosition, Backgrounds[i].position.y, Backgrounds[i].position.z), // Nowa pozycja tła.
                Smoothing * Time.deltaTime); // Określony efekt płynności animacji efektu w grze.
        }

        // Uaktualnienie ruchu kamery.
        _lastPosition = transform.position;
    }
}
