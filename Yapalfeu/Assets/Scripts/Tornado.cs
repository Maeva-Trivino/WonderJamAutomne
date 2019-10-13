using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : Hazard
{


    public void Triggerhazard()
    {
        List<ForestTree> seedTrees = new List<ForestTree>();
        System.Random number = new System.Random();
        Debug.Log("Entree dans Tornado()");

        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.HasSeed())
            {
                Debug.Log("One seed found : ");
                seedTrees.Add(t);
            }
        }
        if (seedTrees.Count > 0)
        {
            bool b = seedTrees[number.Next(0, seedTrees.Count - 1)].RemoveSeed();
            Debug.Log("Un arbre brule => " + b);
        }
        
    }


}
