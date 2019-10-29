using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSource : MonoBehaviour, Interactive
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Select()
    {

    }

    public void Deselect()
    {

    }

    public UserAction GetAction(Player player)
    {
        // TODO : ajuster le temps pour remplir le seau
        if (player.HasEmptyBucket())
        { 
            return new UserAction("Remplir", ActionPriority.MEDIUM, Button.A, new List<Button>() {Button.UP, Button.DOWN }, 3, () => player.FillBucket());
        }
        else
        {
            return null;
        }
    }

}
