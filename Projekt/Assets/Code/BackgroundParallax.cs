using UnityEngine;
using System.Collections;

/// <summary>
/// klasa BackgroundParallax
/// </summary>
public class BackgroundParallax : MonoBehaviour 
{
	/// <summary>
	/// Tablica rzędów elementów tła.
	/// </summary>
    public Transform[] Backgrounds;
	/// <summary>
    /// Wartość efektu poruszania się tła.
	/// </summary>
    public float ParallaxScale;
	/// <summary>
    /// Wartość redukująca stopień poruszania się, dla warstw (rzędów elementów) dalej położonych.
	/// </summary>
    public float ParallaxReductionFactor;
	/// <summary>
    /// Zapewnienie płynności ruchu.
	/// </summary>
    public float Smoothing;

    private Vector3 _lastPosition;

	/// <summary>
	/// Zapamiętanie położenia kamery w ostatniej klatce gry, 
    /// potrzebnego w późniejszym etape do określenia,
	/// o ile tło musi się poruszyć w lewo lub prawo.
	/// </summary>
    
	public void start()
    {
        _lastPosition = transform.position;
    }
	
	/// <summary>
    /// Ruch wykonany od ostaniej klatki do obecnej, przeskalowany przez czynnik efektu
    /// parallax (intensywności ruchu tła).	
	/// </summary>
    public void Update()
    {

        var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;

        /// Dla każdej warstwy (rzędu elementów) tła.
        for (var i = 0; i < Backgrounds.Length; i++)
        {
           /// Zapamiętanie pozycji warstwy tła uaktualnionej o efekt parallax, 
            /// oraz postepującą redukcję tego efektu dla coraz dalszych warstw.
            var backgroundTargetPosition = Backgrounds[i].position.x + parallax * (i * ParallaxReductionFactor + 1);
            /// Ustawienie aktualnej pozycji warstw tła.
            Backgrounds[i].position = Vector3.Lerp(
                Backgrounds[i].position, /// Obecna pozycja tła.
                new Vector3(backgroundTargetPosition, Backgrounds[i].position.y, Backgrounds[i].position.z), /// Nowa pozycja tła.
                Smoothing * Time.deltaTime); /// Określony efekt płynności animacji efektu w grze.
        }

        /// Uaktualnienie ruchu kamery.
        _lastPosition = transform.position;
    }
}
