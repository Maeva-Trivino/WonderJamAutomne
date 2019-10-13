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
        seedGrowDuration = 8f,
        burnDuration = 8f;
    #endregion

    #region Private
    private State state;

    private float stateDuration;

    // Négatif s'il ne brûle pas
    private float burning;

    private Animator animator;

    #endregion

    #region Static
    public static List<ForestTree> trees = new List<ForestTree>();
    #endregion
    #endregion
// Start is called before the first frame update
void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        animator = GetComponent<Animator>();

        if (Random.Range(0, 1000) < 750)
            ChangeState(State.SOIL);
        else
            ChangeState(State.BURNT);

        burning = -1;

        trees.Add(this);
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
                    ChangeState(State.BURNT);
                }
                else
                {
                    ChangeState(State.SOIL);
                }
                UIManager.instance.DeleteTree();
            }
        }

        stateDuration += Time.deltaTime;

        switch (state)
        {
            case State.PLANTED_WET:
                if (stateDuration >= plantedGrowDuration)
                    ChangeState(State.YOUNG_DRY);
                break;
            case State.YOUNG_WET:
                if (stateDuration >= youngGrowDuration)
                {
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
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }
        else
        {
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().sortingOrder = -32768;
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
        gameObject.GetComponent<Renderer>().material.SetInt("_OutlineEnabled", 1);
        gameObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
    }

    public void Deselect()
    {
        gameObject.GetComponent<Renderer>().material.SetInt("_OutlineEnabled", 0);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
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
    public UserAction GetAction(Player player)
    {
        if (IsBurning())
        {
            if (player.HasFilledBucket())
                return new UserAction("Éteindre", Button.A, null, 0, () => { if (player.WaterPlant()) StopBurning(); });
        }
        else
        {
            // TODO : Mettre des combos ou ajuster le temps d'appui
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
                    return new UserAction("Arracher", Button.A, null, 0, () => ChangeState(State.SOIL));
            }
        }

        return null;
    }

    public bool SetOnFire()
    {
        if (!IsBurning() && IsBurnable())
        {
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
    private void StopBurning()
    {
        burning = -1;
        animator.SetBool("isBurning", false);
    }
}
