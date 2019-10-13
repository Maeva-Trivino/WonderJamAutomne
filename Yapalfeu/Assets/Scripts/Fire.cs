using System.Collections.Generic;
using UnityEngine;

public class Fire
{
    public bool Triggerhazard()
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
                if (Random.Range(0,100) <= 25)
                {
                    treesToBurn.Add(t);
                }
                
            }
        }
        if(burnableTrees.Count >= 1 && treesToBurn.Count == 0)
        {
            treesToBurn.Add(burnableTrees[number.Next(0, burnableTrees.Count)]);
        }

        if (treesToBurn.Count > 0)
        {
            HazardAnimationFire.instance.Trigger(treesToBurn);
            return true;
        }
        else
        {
            return false; 
        }
        


    }
}
