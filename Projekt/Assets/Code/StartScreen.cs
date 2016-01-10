using UnityEngine;

/// <summary>
/// Klasa umożliwi ewentualne rozpoczęcie gry od ekranu startowego,
/// a nastepnie przeniesienie do pierwszego etapu gry.
/// </summary>
public class StartScreen : MonoBehaviour
{
    public string FirstLevel;
    public void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        GameManager.Instance.Reset();
        Application.LoadLevel(FirstLevel);
    }
}

