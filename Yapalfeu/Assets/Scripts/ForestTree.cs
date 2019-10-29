using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTree : MonoBehaviour, Interactive
{
    enum State
    {
        SOIL,
        PLANTED_DRY,
        PLANTED_WET,
        YOUNG_DRY,
        YOUNG_WET,
        MATURE,
        BURNT
    }

    #region Variables
    #region Editor
    [SerializeField]
    private static float
        plantedGrowDuration = 3f,
        youngGrowDuration = 5f,
        seedGrowDuration = 10f,
        burnDuration = 8f;

    #region SoundEffects
    //Sound of growing tree
    [SerializeField]
    private AudioSource growTreeSound = null;
    //Sound of burning tree
    [SerializeField]
    private AudioSource burningTreeSound = null;
    //Sound of throwing a tree
    [SerializeField]
    private AudioSource throwTreeSound = null;
    #endregion
    #endregion

    #region Private
    private State state;

    private float stateDuration;

    // Négatif s'il ne brûle pas
    private float burning;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    #endregion

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        animator = GetComponentInChildren<Animator>();

        if (Random.Range(0, 1000) < 750)
            ChangeState(State.SOIL);
        else
            ChangeState(State.BURNT);

        burning = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsBurning())
        {
            burning += Time.deltaTime;

            if (burning >= burnDuration)
            {
                if(state == State.MATURE)
                {
                    UIManager.instance.DeleteTree();
                    ChangeState(State.BURNT);
                }
                else
                {
                    ChangeState(State.SOIL);
                }
            }
        }

        stateDuration += Time.deltaTime;

        switch (state)
        {
            case State.PLANTED_WET:
                if (stateDuration >= plantedGrowDuration)
                {
                    growTreeSound.Play();
                    ChangeState(State.YOUNG_DRY);
                }
                break;
            case State.YOUNG_WET:
                if (stateDuration >= youngGrowDuration)
                {
                    growTreeSound.Play();
                    ChangeState(State.MATURE);
                    UIManager.instance.AddTree();
                }
                break;
            case State.MATURE:
                if (HasSeed())
                    animator.SetBool("hasSeed", true);
                else
                    animator.SetBool("hasSeed", false);
                break;
        }
    }

    private void ChangeState(State s)
    {
        stateDuration = 0;
        state = s;

        if (s == State.SOIL || s == State.BURNT)
        {
            StopBurning();
        }
        
        if (state != State.SOIL)
        {
            GetComponent<Collider2D>().isTrigger = false;
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }
        else
        {
            GetComponent<Collider2D>().isTrigger = true;
            spriteRenderer.sortingOrder = -32768;
        }

        switch (state)
        {
            case State.SOIL:
                animator.SetInteger("growState", 0);
                break;
            case State.PLANTED_DRY:
            case State.PLANTED_WET:
                animator.SetInteger("growState", 1);
                break;
            case State.YOUNG_DRY:
            case State.YOUNG_WET:
                animator.SetInteger("growState", 2);
                break;
            case State.MATURE:
                animator.SetInteger("growState", 3);
                break;
            case State.BURNT:
                animator.SetInteger("growState", 4);
                break;
        }
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

    public bool HasSeed()
    {
        return state == State.MATURE && stateDuration >= seedGrowDuration;
    }

    private bool BurnSeed()
    {
        if (HasSeed())
        {
            stateDuration = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveSeed()
    {
        if(HasSeed())
        {
            stateDuration = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsBurning()
    {
        return burning >= 0;
    }

    public bool IsBurnable()
    {
        return state != State.BURNT && state != State.SOIL && state != State.PLANTED_DRY && state != State.PLANTED_WET ;
    }

    public bool IsDrownable()
    {
        return state == State.PLANTED_DRY || state == State.PLANTED_WET || state == State.YOUNG_DRY || state == State.YOUNG_WET ;
    }

    public bool Drown()
    {
        if (IsDrownable())
        {
            ChangeState(State.SOIL);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsPlant()
    {
        return state == State.PLANTED_DRY || state == State.PLANTED_WET;
    }

    public bool IsAlive()
    {
        return state != State.SOIL && state != State.BURNT;
    }

    public UserAction GetAction(Player player)
    {
        if (IsBurning())
        {
            if (player.HasFilledBucket())
                return new UserAction("Éteindre", Button.A, null, 0, () => { if (player.ExtinguishFire()) StopBurning(); });
        }
        else
        {
            switch (state)
            {
                case State.SOIL:
                    if (player.HasSeed())
                        return new UserAction("Planter", Button.A, null, 0, () =>
                        {
                            if (player.PlantSeed())
                                ChangeState(State.PLANTED_DRY);
                        });
                    break;
                case State.PLANTED_DRY:
                    if (player.HasFilledBucket())
                        return new UserAction("Arroser", Button.A, null, 0, () =>
                        {
                            if (player.WaterPlant()) ChangeState(State.PLANTED_WET);
                        });
                    break;
                case State.YOUNG_DRY:
                    if (player.HasFilledBucket())
                        return new UserAction("Arroser", Button.A, null, 0, () =>
                        {
                            if (player.WaterPlant()) ChangeState(State.YOUNG_WET);
                        });
                    break;
                case State.MATURE:
                    if (HasSeed())
                        return new UserAction("Récolter", Button.A, new List<Button> () {Button.LEFT, Button.RIGHT } , 3, () =>
                        {
                            player.HarvestSeed();
                            stateDuration = 0;
                        });
                    break;
                case State.BURNT:
                    return new UserAction("Arracher", Button.A, null, 0, () => RemoveTree());
            }
        }

        return null;
    }

    private void RemoveTree()
    {
        throwTreeSound.Play();
        ChangeState(State.SOIL);
    }

    public bool SetOnFire()
    {
        if (!IsBurning() && IsBurnable())
        {
            burningTreeSound.Play();
            burning = 0;
            BurnSeed();
            animator.SetBool("isBurning", true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StopBurning()
    {
        burning = -1;
        animator.SetBool("isBurning", false);
    }
}
