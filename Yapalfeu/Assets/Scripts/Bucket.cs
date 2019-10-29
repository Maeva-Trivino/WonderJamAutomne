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
    private Sprite emptyBucketSprite = null;
    [SerializeField]
    private Sprite filledBucketSprite = null;

    private State state;

    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        state = State.EMPTY;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = emptyBucketSprite;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }

    public void Empty()
    {
        state = State.EMPTY;
        spriteRenderer.sprite = emptyBucketSprite;
    }

    public void Fill()
    {
        state = State.FILLED;
        spriteRenderer.sprite = filledBucketSprite;
    }

    public bool isFilled()
    {
        return state == State.FILLED;
    }

    public void Select()
    {
        spriteRenderer.material.SetInt("_OutlineEnabled", 1);
        spriteRenderer.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
    }

    public void Deselect()
    {
        spriteRenderer.material.SetInt("_OutlineEnabled", 0);
        spriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
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
            if (player.GetBucket().state != this.state)
            {
                return new UserAction("Échanger", Button.A, null, 0, () =>
                {
                    if (player.GetBucket().state != this.state)
                    {
                        if (this.isFilled())
                        {
                            this.Empty();
                            player.ExchangeBucket(false);
                        }
                        else
                        {
                            this.Fill();
                            player.ExchangeBucket(true);
                        }
                    }
                }, 0);
            }
            else
            {
                return null;
            }
        }
    }

    // Pose le seau au sol à l'emplacement passé en paramètres
    public void SetOnGround(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

}
