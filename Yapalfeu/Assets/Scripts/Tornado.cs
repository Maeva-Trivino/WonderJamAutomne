using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado
{
    public bool Triggerhazard()
    {
        List<ForestTree> seedTrees = new List<ForestTree>();

        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Tree"))
        {
            ForestTree t = o.GetComponent<ForestTree>();
            if (t.HasSeed() || t.IsPlant())
            {
                seedTrees.Add(t);
            }
        }
        if (seedTrees.Count > 0)
        {
            HazardAnimationTornado.instance.Trigger(seedTrees);
            return true;
        }
        else
        {
            return false;
        }

    }
}
