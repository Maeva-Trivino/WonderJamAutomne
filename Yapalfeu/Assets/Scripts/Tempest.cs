using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempest
{

    public bool Triggerhazard()
    {
        List<ForestTree> drownableTrees = new List<ForestTree>();
        List<ForestTree> burnableTrees = new List<ForestTree>();
        System.Random number = new System.Random();
        Debug.Log("Entree dans Tempest()");

        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.IsDrownable())
            {
                drownableTrees.Add(t);
            }

            if (t.IsBurnable())
            {
                burnableTrees.Add(t);
            }
        }
        if (drownableTrees.Count > 0)
        {
            HazardAnimationTempest.instance.Trigger(drownableTrees,burnableTrees);
            return true;
        }
        else
        {
            return false;
        }


    }
}
