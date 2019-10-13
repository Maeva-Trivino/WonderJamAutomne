using System.Collections.Generic;
using UnityEngine;

public class Fire : Hazard
{
    public void Triggerhazard()
    {
        List<ForestTree> burnableTrees = new List<ForestTree>();
        System.Random number = new System.Random();
        Debug.Log("Entree dans Fire()");
        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.IsBurnable()) 
            {
                burnableTrees.Add(t);
            }
        }
        if (burnableTrees.Count > 0)
        {
            
            bool b  = burnableTrees[number.Next(0, burnableTrees.Count - 1)].SetOnFire();
            Debug.Log("Un arbre brule => " + b);
        }
            
    }
}
