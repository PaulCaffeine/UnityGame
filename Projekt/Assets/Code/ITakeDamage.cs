using UnityEngine;

/// <summary>
/// Interfejs posiadaj�cy funkcj� przyjmowania obra�e�
/// o podanej warto�ci, zadanych przez dany obiekt.
/// </summary>
public interface ITakeDamage
{
	void TakeDamage(int damage, GameObject instigator);
}

