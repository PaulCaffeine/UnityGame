using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Instancja LevelManagera.
    /// </summary>
    public static LevelManager Instance { get; private set; }

    /// <summary>
    /// Gracz.
    /// </summary>
    public Player Player { get; private set; }
    /// <summary>
    /// Kamera.
    /// </summary>
    public CameraController Camera { get; private set; }
    /// <summary>
    /// Upływ czasu od ostatniej wartości _started.
    /// </summary>
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }

    /// <summary>
    /// Obliczenie bonusu czasowego za osiągnięcie nowego checkpointu w wyznaczonym czasie.
    /// </summary>
    public int CurrentTimeBonus
    {
        get
        {
            var secondDifference = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDifference) * BonusSecondMultiplier;
        }
    }

    /// <summary>
    /// Lista checkpointów.
    /// </summary>
    private List<Checkpoint> _checkpoints;
    /// <summary>
    /// Indeks checkpointu, w którym gracz się znajduje.
    /// </summary>
    private int _currentCheckpointIndex;
    /// <summary>
    /// Wartość wyznaczająca upływ czasu od danego wydarzenia (spawnu/respawnu gracza).
    /// </summary>
    private DateTime _started;
    /// <summary>
    /// Przechowuje liczbę punktów gracza.
    /// </summary>
    private int _savedPoints;

    /// <summary>
    /// Checkpoint związany z spawnowaniem gracza w edytorze Unity (jeśli go ustawimy w Unity).
    /// </summary>
    public Checkpoint DebugSpawn;
    /// <summary>
    /// Czas na osiągnięcie nowego checkpointu.
    /// </summary>
    public int BonusCutoffSeconds;
    /// <summary>
    /// Mnożnik dotyczący zdobotych punktów za dotarcie do checkpointu w wyznaczonym czasie.
    /// </summary>
    public int BonusSecondMultiplier;

    /// <summary>
    /// Ustawienie instancji klasy, do której można się później odwoływać w innych klasach.
    /// </summary>
    public void Awake()
    {
        _savedPoints = GameManager.Instance.Points;
        Instance = this;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        /// Posortowana lista checkpointów.
        _checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
        /// Jeśli nie ma checkpointów, ustaw flagę na -1, w przeciwnym wypadku ustaw indeks na 0.
        _currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;

        /// Gracz i kamera są teraz w lokalnej pamięci podręcznej (cache) LevelManagera.
        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraController>();

        /// Ustawienie wartości początkowej _started na aktualny czas.
        _started = DateTime.UtcNow;

        /// Przypisanie obiektów (gwiazdek) do poszczególnych checkpointów (patrząc wstecz od danego checkpointu).
        var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (var listener in listeners)
        {
            for (var i = _checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;

                _checkpoints[i].AssignObjectToCheckpoint(listener);
                break;
            }

        }

/// Wykonywane tylko w edytorze Unity, ustawia domyślny punkt spawnu gracza jako DebugSpawn,
/// jeśli ustawiony jest DebugSpawn (nie ma wartości null).
#if UNITY_EDITOR
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#else
        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#endif
    }

    /// <summary>
    /// Aktualizowanie pozycji gracza względem checkpointów, liczby punktów i upływu czasu od osiągnięcia ostatniego checkpointu.
    /// </summary>
    public void Update()
    {
        /// Sprawdzamy czy jesteśmy na ostatnim checkpoincie (jeśli tak,
        /// to nie ma sensu wykonywać dalszych operacji - nie ma dalszych checkpointów).
        var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
        if (isAtLastCheckpoint)
            return;

        /// Sprawdzamy czy osiągnęliśmy nowy checkpoint, jeśli nie, wracamy.
        var distanceToNextCheckpoint = _checkpoints[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckpoint >= 0)
            return;

        /// Kod wykonywany, gdy osiągnięto nowy checkpoint.
        /// Opuszczenie starego checkpointu.
        _checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();
        /// Inkrementacja wartości indeksu obecnego checkpointu.
        _currentCheckpointIndex++;
        /// Wejście w nowy checkpoint. 
        _checkpoints[_currentCheckpointIndex].PlayerHitCheckpoint();

        /// Dodanie ewentualnego bonusu czasowego.
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        /// Zapisanie liczby punktów.
        _savedPoints = GameManager.Instance.Points;
        /// Zresetowanie czasu w _started.
        _started = DateTime.UtcNow;
    }

    /// <summary>
    /// Metoda uruchamiająca procedurę przejścia do następnego poziomu.
    /// </summary>
    /// <param name="levelName"></param>
    public void GotoNextLevel(string levelName)
    {
        StartCoroutine(GotoNextLevelCo(levelName));
    }

    /// <summary>
    /// Procedura przejścia do nastepnego poziomu.
    /// </summary>
    /// <param name="levelName"></param>
    /// <returns></returns>
    private IEnumerator GotoNextLevelCo(string levelName)
    {
        /// Odwołanie się do metody FinishLevel gracza, która 
        /// tymczasowo deaktywuje część jego właściwości.
        Player.FinishLevel();
        /// Dodanie punktów premii czasowej.
        GameManager.Instance.AddPoints(CurrentTimeBonus);

        /// Wyświetlenie komunikatu ukończenia poziomu.
        FloatingText.Show("Level Complete!", "CheckpointText", new CenteredTextPositioner(.2f));
        yield return new WaitForSeconds(1);

        /// Wyświetlenie ilości zdobytych punktów.
        FloatingText.Show(string.Format("{0} points!", GameManager.Instance.Points), "CheckpointText", new CenteredTextPositioner(.1f));
        yield return new WaitForSeconds(5f);

        /// Wczytanie następnego poziomu.
        if (string.IsNullOrEmpty(levelName))
            Application.LoadLevel("StartScreen");
        else
            Application.LoadLevel(levelName);
    }

    /// <summary>
    /// Uruchomienie procedury zabicia gracza.
    /// </summary>
    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }

    /// <summary>
    /// Procedura zabicia gracza.
    /// </summary>
    /// <returns></returns>
    private IEnumerator KillPlayerCo()
    {
        /// Zabicie gracza.
        Player.Kill();
        /// Kamera nie śledzi gracza.
        Camera.IsFollowing = false;
        yield return new WaitForSeconds(2f);
        /// Po 2 sekundach kamera znów śledzi gracza.
        Camera.IsFollowing = true;

        /// Gracz respawnuje się na ostatnim checkpoincie.
        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);

        /// Ustawienie wartości _started na aktualny czas.
        _started = DateTime.UtcNow;
        /// Gracz posiada (otrzymuje) liczbę punktów zapisaną wcześniej w _savedPoints.
        GameManager.Instance.ResetPoints(_savedPoints);
    }
}


