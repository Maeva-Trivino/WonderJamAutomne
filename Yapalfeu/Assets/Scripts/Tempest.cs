using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempest : Hazard
{

    public void Triggerhazard()
    {
        List<ForestTree> DrownableTrees = new List<ForestTree>();
        System.Random number = new System.Random();

        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.IsDrownable())
            {
                DrownableTrees.Add(t);
            }
        }
        DrownableTrees[number.Next(0, DrownableTrees.Count)].Drown();
    }
}
