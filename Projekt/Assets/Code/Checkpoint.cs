using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Klasa odpowiedzialna za respawn gracza w danym checkpoincie.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private List<IPlayerRespawnListener> _listeners;

    /// <summary>
    /// Tworzenie listy listenerów obsługujących respawn obiektów wraz z graczem w kolejnych checkpointach.
    /// </summary>
    public void Awake()
    {
        _listeners = new List<IPlayerRespawnListener>();
    }

    /// <summary>
    /// Uruchomienie metody PlayerHitCheckpointCo,
    /// z aktualną wartością bonusu czasowego jako argumentem.
    /// </summary>
    public void PlayerHitCheckpoint()
    {
        StartCoroutine(PlayerHitCheckpointCo(LevelManager.Instance.CurrentTimeBonus));
    }

    /// <summary>
    /// Po wejściu gracza w nowy checkpoint, wyświetlany jest odpowiedni
    /// komunikat o zdarzeniu oraz bonusie czasowym.
    /// </summary>
    /// <param name="bonus"></param>
    /// <returns></returns>
    private IEnumerator PlayerHitCheckpointCo(int bonus)
    {

        FloatingText.Show("Checkpoint!", "CheckpointText", new CenteredTextPositioner(.5f));
        yield return new WaitForSeconds(.5f);
        FloatingText.Show(string.Format("+{0} time bonus!", bonus), "CheckpointText", new CenteredTextPositioner(0.5f));

    }

    /// <summary>
    /// Metoda wywoływana w razie opuszczenia checkpointu
    /// (nic nie robi).
    /// </summary>
    public void PlayerLeftCheckpoint()
    {

    }

    /// <summary>
    /// Respawnowanie gracza w miejscu, w którym
    /// znajduje się checkpoint.
    /// </summary>
    /// <param name="player"></param>
    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform);

        foreach (var listener in _listeners)
            listener.OnPlayerRespawnInThisCheckpoint(this, player);
    }

    /// <summary>
    /// Przydzielanie obiektów do checkpointów, które mają się
    /// zrespawnować po respawnie gracza.
    /// </summary>
    /// <param name="listener"></param>
    public void AssignObjectToCheckpoint(IPlayerRespawnListener listener)
    {
        _listeners.Add(listener);
    }
}


