using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void Deselect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public Action GetAction(Player player)
    {
        // TODO : ajuster le temps pour remplir le seau
        if (player.HasEmptyBucket())
        {
            return new Action("Remplir", Button.A, null, 0, Config.actionDuration, () => player.FillBucket());
        }
        else
        {
            return null;
        }
    }

}
