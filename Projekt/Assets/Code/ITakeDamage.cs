using UnityEngine;

/// <summary>
/// Interfejs posiadaj¹cy funkcjê przyjmowania obra¿eñ
/// o podanej wartoœci, zadanych przez dany obiekt.
/// </summary>
public interface ITakeDamage
{
	void TakeDamage(int damage, GameObject instigator);
}

