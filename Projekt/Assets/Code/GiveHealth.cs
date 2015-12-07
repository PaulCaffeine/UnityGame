using UnityEngine;

public class GiveHealth : MonoBehaviour, IPlayerRespawnListener
{
    
    public GameObject Effect;
    public int HealthToGive;

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        player.GiveHealth(HealthToGive, gameObject);
        Instantiate(Effect, transform.position, transform.rotation);

        gameObject.SetActive(false);

        //FloatingText.Show(string.Format("+{0}!", HealthToGive), "GiveHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
    }

    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
    
}

