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
        gameObject.GetComponent<Tilemap>().color = Color.blue;
    }

    public void Deselect()
    {
        gameObject.GetComponent<Tilemap>().color = Color.white;
    }

    public Action GetAction(Player player)
    {
        // TODO : ajuster le temps pour remplir le seau
        if (player.HasEmptyBucket())
        { 
            return new Action("Remplir", Button.A, new List<Button>() {Button.UP, Button.DOWN }, 3, () => player.FillBucket());
        }
        else
        {
            return null;
        }
    }

}
