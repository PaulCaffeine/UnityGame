/// <summary>
/// Interfejs odpowiedzialny za zrespawnowanie gracza w danym checkpoincie.
/// </summary>

public interface IPlayerRespawnListener
{
    void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player);
}
