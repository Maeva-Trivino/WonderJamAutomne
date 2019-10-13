using System.Collections.Generic;
using UnityEngine;

public class Fire : Hazard
{
    public void Triggerhazard()
    {
        List<ForestTree> burnableTrees = new List<ForestTree>();
        List<ForestTree> treesToBurn= new List<ForestTree>();

        System.Random number = new System.Random();
        Debug.Log("Entree dans Fire()");
        foreach (ForestTree t in ForestTree.trees)
        {
            if (t.IsBurnable()) 
            {
                burnableTrees.Add(t);
                if (number.Next(1, 100) > 25)
                {
                    treesToBurn.Add(t);
                }
                
            }
        }
        if(burnableTrees.Count >= 1 && treesToBurn.Count == 0)
        {
            treesToBurn.Add(burnableTrees[number.Next(0, burnableTrees.Count)]);
        }

        if (burnableTrees.Count > 0)
        {
            
            //bool b  = burnableTrees[number.Next(0, burnableTrees.Count - 1)].SetOnFire();
            HazardAnimationFire.instance.Trigger(treesToBurn);
        }
        


    }
}
