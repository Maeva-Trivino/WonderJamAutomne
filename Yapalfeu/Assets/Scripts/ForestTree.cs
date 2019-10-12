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
        MATURE, // TODO when do we drop seeds?
        BURNT
    }

    #region Variables
    #region Editor
    [SerializeField]
    private static float
        plantedGrowDuration = 3f,
        youngGrowDuration = 3f,
        seedGrowDuration = 10f,
        burnDuration = 3f;
    #endregion

    #region Private
    private State state;

    private float stateDuration;

    // Négatif s'il ne brûle pas
    private float burning;
    #endregion

    #region Static
    public static List<ForestTree> trees = new List<ForestTree>();
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        state = State.SOIL;
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
                ChangeState(State.BURNT);
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
        }
    }

    private void ChangeState(State s)
    {
        stateDuration = 0;
        state = s;
        if(s==State.YOUNG_DRY)
            GetComponent<Collider2D>().isTrigger = false;
    }

    public void Select()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void Deselect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool HasSeed()
    {
        return state == State.MATURE && stateDuration >= seedGrowDuration;
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

    private void StopBurning()
    {
        burning = -1;
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
    public Action GetAction(Player player)
    {
        if (IsBurning())
        {
            if (player.HasFilledBucket())
                return new Action("Éteindre", Button.A, null, 0, () => { if (player.WaterPlant()) StopBurning(); });
        }
        else
        {
            // TODO : Mettre des combos ou ajuster le temps d'appui
            switch (state)
            {
                case State.SOIL:
                    if (player.HasSeed())
                        return new Action("Planter", Button.A, null, 0, () =>
                        {
                            if (player.PlantSeed())
                                ChangeState(State.PLANTED_DRY);
                        });
                    break;
                case State.PLANTED_DRY:
                    if (player.HasFilledBucket())
                        return new Action("Arroser", Button.A, null, 0, () =>
                        {
                            if (player.WaterPlant()) ChangeState(State.PLANTED_WET);
                        });
                    break;
                case State.YOUNG_DRY:
                    if (player.HasFilledBucket())
                        return new Action("Arroser", Button.A, null, 0, () =>
                        {
                            if (player.WaterPlant()) ChangeState(State.YOUNG_WET);
                        });
                    break;
                case State.MATURE:
                    if (HasSeed())
                        return new Action("Récolter", Button.A, new List<Button> () {Button.LEFT, Button.RIGHT } , 3, () =>
                        {
                            player.HarvestSeed();
                            stateDuration = 0;
                        });
                    break;
                case State.BURNT:
                    return new Action("Arracher", Button.A, null, 0, () => ChangeState(State.SOIL));
            }
        }

        return null;
    }

    public bool SetOnFire()
    {
        if (burning < 0)
        {
            burning = 0;
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
