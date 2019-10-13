using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempest : Hazard
{

    public void Triggerhazard()
    {
        List<ForestTree> drownableTrees = new List<ForestTree>();
        System.Random number = new System.Random();
        Debug.Log("Entree dans Tempest()");

        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.IsDrownable())
            {
                drownableTrees.Add(t);
            }
        }
        if (drownableTrees.Count > 0)
        {
            //bool b =drownableTrees[number.Next(0, drownableTrees.Count - 1)].Drown();
            HazardAnimationTempest.instance.Trigger(drownableTrees);
        }


    }
}
