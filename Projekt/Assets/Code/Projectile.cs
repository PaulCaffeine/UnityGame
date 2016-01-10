using UnityEngine;

/// <summary>
/// Podstawowa klasa pocisku.
/// </summary>
public abstract class Projectile : MonoBehaviour {
    /// <summary>
    /// Prędkość pocisku.
    /// </summary>
	public float Speed;
    /// <summary>
    /// Warstwa kolizji pocisku z otoczeniem.
    /// </summary>
	public LayerMask CollisionMask;

    /// <summary>
    /// Obiekt, który wystrzeli pocisk.
    /// </summary>
	public GameObject Owner { get; private set;}
    /// <summary>
    /// Kierunek strzału.
    /// </summary>
	public Vector2 Direction { get; private set;}
    /// <summary>
    /// Prędkość obiektu-właściciela pocisku, która później jest dodawana do prędkości pocisku podczas wystrzału.
    /// </summary>
	public Vector2 InitialVelocity { get; private set; }

    /// <summary>
    /// Inicjalizacja pocisku, pobierająca kierunek strzału, prędkość obiektu strzelającego,
    /// oraz obiekt strzelający jako parametry.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="direction"></param>
    /// <param name="initialVelocity"></param>
	public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity)
	{
        /// Ustawienie kierunku pocisku.
        transform.right = direction;
        /// Ustawienie obiektu oddającego strzał.
		Owner = owner;
        /// Ustawienie kierunku strzału.
		Direction = direction;
        /// Ustawienie początkowej prędkości strzału.
		InitialVelocity = initialVelocity;
		OnInitialized();
	}

    /// <summary>
    /// Pusta metoda, którą mogą zaimplementować klasy potomne.
    /// </summary>
	public virtual void OnInitialized()
	{

	}

    /// <summary>
    /// Metoda wywoływana po zderzeniu pocisku z innym obiektem.
    /// </summary>
    /// <param name="other"></param>
	public virtual void OnTriggerEnter2D(Collider2D other)
	{
        /// Pocisk zderzył się z czymś, co nie odpowiada masce kolizji.
        if ((CollisionMask.value & (1 << other.gameObject.layer)) == 0) {
			OnNotCollideWith(other);
			return;
		}

        /// Pocisk zderzył się z obiektem, który wystrzelił pocisk.
		var isOwner = other.gameObject == Owner;
		if (isOwner) {
			OnCollideOwner();
			return;
		}

        /// Pocisk zderzył się z obiektem, który może przyjmować obrażenia.
		var takeDamage = (ITakeDamage) other.GetComponent(typeof (ITakeDamage));
		if (takeDamage != null) {
			OnCollideTakeDamage(other, takeDamage);
			return;
		}

        /// Żadne z powyższych.
		OnCollideOther (other);
	}

    /// <summary>
    /// Pusta metoda, którą mogą zaimplementować klasy potomne.
    /// </summary>
    /// <param name="other"></param>
	protected virtual void OnNotCollideWith(Collider2D other)
	{

	}
    /// <summary>
    /// Pusta metoda, którą mogą zaimplementować klasy potomne.
    /// </summary>
	protected virtual void OnCollideOwner()
	{

	}
    /// <summary>
    /// Pusta metoda, którą mogą zaimplementować klasy potomne.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="takeDamage"></param>
	protected virtual void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
	{

	}
    /// <summary>
    /// Pusta metoda, którą mogą zaimplementować klasy potomne.
    /// </summary>
    /// <param name="other"></param>
	protected virtual void OnCollideOther(Collider2D other)
	{

	}
}
