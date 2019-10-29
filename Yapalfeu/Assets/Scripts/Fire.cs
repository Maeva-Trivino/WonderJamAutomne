using System.Collections.Generic;
using UnityEngine;

public class Fire
{
    public bool Triggerhazard()
    {
        List<ForestTree> burnableTrees = new List<ForestTree>();
        List<ForestTree> treesToBurn= new List<ForestTree>();

        Debug.Log("Entree dans Fire()");
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Tree"))
        {
            ForestTree t = o.GetComponent<ForestTree>();
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
            treesToBurn.Add(burnableTrees[Random.Range(0, burnableTrees.Count)]);
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
