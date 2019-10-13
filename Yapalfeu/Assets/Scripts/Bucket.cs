using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour, Interactive
{
    enum State
    {
        EMPTY,
        FILLED
    }
    
    [SerializeField]
    private Sprite bucket_sprite_empty = null;
    [SerializeField]
    private Sprite bucket_sprite_filled = null;

    State state;

    // Start is called before the first frame update
    void Start()
    {
        state = State.EMPTY;
        GetComponent<SpriteRenderer>().sprite = bucket_sprite_empty;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Empty()
    {
        state = State.EMPTY;
        GetComponent<SpriteRenderer>().sprite = bucket_sprite_empty;
    }

    public void Fill()
    {
        state = State.FILLED;
        GetComponent<SpriteRenderer>().sprite = bucket_sprite_filled;
    }

    public bool isFilled()
    {
        return state == State.FILLED;
    }

    public void Select()
    {
        gameObject.GetComponent<Renderer>().material.SetInt("_OutlineEnabled", 1);
        gameObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
    }

    public void Deselect()
    {
        gameObject.GetComponent<Renderer>().material.SetInt("_OutlineEnabled", 0);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public UserAction GetAction(Player player)
    {
        if (!player.HasBucket())
        {
            return new UserAction("Prendre", Button.A, null, 0, () =>
            {
                if (player.PickUpBucket(this)) 
                    gameObject.SetActive(false);
            }, 0);
        }
        else
        {
            return null;
        }
    }

    // Pose le seau au sol à l'emplacement passé en paramètres
    public void SetOnGround(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

}
