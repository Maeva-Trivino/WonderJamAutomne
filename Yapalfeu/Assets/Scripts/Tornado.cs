using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado
{


    public bool Triggerhazard()
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
            HazardAnimationTornado.instance.Trigger(seedTrees);
            return true;
        }else
        {
            return false;
        }

    }


}
