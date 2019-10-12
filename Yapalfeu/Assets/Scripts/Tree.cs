using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, Interactive
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
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void Deselect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public Action GetAction(PlayerState state)
    {
        return new Action("Planter", Button.A, null, 0, 0);
    }

    public void DoAction(PlayerState state)
    {
        throw new System.NotImplementedException();
    }
}
