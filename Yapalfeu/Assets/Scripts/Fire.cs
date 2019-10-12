using System.Collections.Generic;
using UnityEngine;

public class Fire : Hazard
{
    public void Triggerhazard()
    {
        List<ForestTree> burnableTrees = new List<ForestTree>();
        System.Random number = new System.Random();
 
        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.IsBurning()) 
            {
                burnableTrees.Add(t);
            }
        }

        burnableTrees[number.Next(0, burnableTrees.Count)].SetOnFire();
    }
}
