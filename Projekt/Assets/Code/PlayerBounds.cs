using UnityEngine;

/// <summary>
/// Klasa ograniczająca mozliwy ruch gracza jedynie 
/// w zakresie widzialnego istniejącego poziomu.
/// </summary>
public class PlayerBounds : MonoBehaviour
{
    /// <summary>
    /// Możliwe opcje akcji po przekroczeniu przez gracza dozwolonych granic poziomu,
    /// czyli nie robienie niczego, śmierć, lub ograniczenie ruchu.
    /// </summary>
    public enum BoundsBehavior
    {
        Nothing,
        Constrain,
        Kill
    }

    public BoxCollider2D Bounds;
    public BoundsBehavior Above;
    public BoundsBehavior Below;
    public BoundsBehavior Left;
    public BoundsBehavior Right;

    private Player _player;
    private BoxCollider2D _boxCollider;

    /// <summary>
    /// Inicjalizacja obiektu gracza i box collidera.
    /// </summary>
    public void Start()
    {
        _player = GetComponent<Player>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Korygowanie położenia gracza po przekroczeniu granic poziomu, uwzględniając granice:
    /// górną, dolną, po prawej stronie ekranu oraz po lewej stronie ekranu.
    /// </summary>
    public void Update()
    {
        if (_player.IsDead)
            return;

        var colliderSize = new Vector2(_boxCollider.size.x * Mathf.Abs(transform.localScale.x), _boxCollider.size.y * Mathf.Abs(transform.localScale.y)) / 2;

        if (Above != BoundsBehavior.Nothing && transform.position.y + colliderSize.y > Bounds.bounds.max.y)
        {
            ApplyBoundsBehavior(Above, new Vector2(transform.position.x, Bounds.bounds.max.y - colliderSize.y));
        }

        if (Below != BoundsBehavior.Nothing && transform.position.y - colliderSize.y < Bounds.bounds.min.y)
        {
            ApplyBoundsBehavior(Below, new Vector2(transform.position.x, Bounds.bounds.min.y + colliderSize.y));
        }

        if (Right != BoundsBehavior.Nothing && transform.position.x + colliderSize.x > Bounds.bounds.max.x)
        {
            ApplyBoundsBehavior(Right, new Vector2(Bounds.bounds.max.x - colliderSize.x, transform.position.y));
        }

        if (Left != BoundsBehavior.Nothing && transform.position.x - colliderSize.x < Bounds.bounds.min.x)
        {
            ApplyBoundsBehavior(Left, new Vector2(Bounds.bounds.min.x + colliderSize.x, transform.position.y));
        }
    }

    /// <summary>
    /// Funkcja zabijająca gracza lub korygująca jego położenie,
    /// w zależności od wartości parametru behavior typu BoundsBehavior.
    /// </summary>
    /// <param name="behavior"></param>
    /// <param name="constrainedPosition"></param>
    private void ApplyBoundsBehavior(BoundsBehavior behavior, Vector2 constrainedPosition)
    {
        if (behavior == BoundsBehavior.Kill)
        {
            LevelManager.Instance.KillPlayer();
            return;
        }

        transform.position = constrainedPosition;
    }
}

