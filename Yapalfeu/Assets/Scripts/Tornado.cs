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
        if (seedTrees.Count > 0)
        {
            //bool b = seedTrees[number.Next(0, seedTrees.Count - 1)].RemoveSeed();
            HazardAnimationTornado.instance.Trigger(seedTrees);

        }

    }


}
