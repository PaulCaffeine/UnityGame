using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// klasa PathDefinition
/// </summary>

public class PathDefinition : MonoBehaviour 
{
    /// Punkty sciezki
    public Transform[] Points;

    /// Enumerator punktow w sciezce. 
    /// Zapewnia poruszanie sie np. platform w obie strony po sciezce.
    public IEnumerator<Transform> GetPathEnumerator()
    {
        if (Points == null || Points.Length < 1)
            yield break;
        var direction = 1;
        var index = 0;
        while (true)
        {
            yield return Points[index];

            if (Points.Length == 1)
                continue;

            if (index <= 0)
                direction = 1;
            else if (index >= Points.Length - 1)
                direction = -1;

            index = index + direction;
        }
    }

    /// Rysowanie linii miedzy punktami sciezki w Scene view,
    /// upewniajac sie czy istnieje odpowiednia ilosc punktow
    /// i omijajac usuniete.
    public void OnDrawGizmos()
    {
        if (Points == null || Points.Length < 2)
            return;
        
        var points = Points.Where(t => t != null).ToList();
        if (points.Count < 2)
            return;
               
        for (var i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i-1].position, points[i].position);
        }
    }
}
