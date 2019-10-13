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

    private State state;

    private SpriteRenderer renderer;
    [SerializeField]
    private AudioSource fillBucket;

    //Sound of getting the bucket
    [SerializeField]
    private AudioSource getBucketSound;

    // Start is called before the first frame update
    void Start()
    {
        state = State.EMPTY;
        renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = bucket_sprite_empty;
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }

    public void Empty()
    {
        state = State.EMPTY;
        renderer.sprite = bucket_sprite_empty;
    }

    public void Fill()
    {
        fillBucket.Play();
        state = State.FILLED;
        renderer.sprite = bucket_sprite_filled;
    }

    public bool isFilled()
    {
        return state == State.FILLED;
    }

    public void Select()
    {
        renderer.material.SetInt("_OutlineEnabled", 1);
        renderer.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
    }

    public void Deselect()
    {
        renderer.material.SetInt("_OutlineEnabled", 0);
        renderer.transform.localScale = new Vector3(1f, 1f, 1f);
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
            return new UserAction("Echanger", Button.A, null, 0, () =>
            {
                if (player.GetBucket().state != this.state)
                {
                    if (this.isFilled())
                    {
                        this.Empty();
                        player.FillBucket();
                    } else
                    {
                        this.Fill();
                        player.WaterPlant();
                    }
                }
                
             

            }, 0);
        }
    }

    // Pose le seau au sol à l'emplacement passé en paramètres
    public void SetOnGround(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

}
