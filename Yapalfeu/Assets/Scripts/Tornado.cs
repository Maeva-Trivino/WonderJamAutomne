using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : Hazard
{


    public void Triggerhazard()
    {
        List<ForestTree> seedTrees = new List<ForestTree>();
        System.Random number = new System.Random();

        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.HasSeed())
            {
                seedTrees.Add(t);
            }
        }
        seedTrees[number.Next(0, seedTrees.Count)].RemoveSeed();
    }


}
