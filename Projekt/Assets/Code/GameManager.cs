using UnityEngine;

/// <summary>
/// Klasa odpowiedzialna za naliczanie i resetowanie punktów podczas gry.
/// </summary>
public class GameManager
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance ?? (_instance = new GameManager()); } }

    public int Points { get; private set; }

    private GameManager()
    {

    }

    /// <summary>
    /// Wyzerowanie punktów.
    /// </summary>
    public void Reset()
    {
        Points = 0;
    }

    /// <summary>
    /// Ustawienie ilości punktów na wartość podaną w parametrze.
    /// </summary>
    /// <param name="points"></param>
    public void ResetPoints(int points)
    {
        Points = points;
    }

    /// <summary>
    /// Dodanie punktów.
    /// </summary>
    /// <param name="pointsToAdd"></param>
    public void AddPoints(int pointsToAdd)
    {
        Points += pointsToAdd;
    }
}


