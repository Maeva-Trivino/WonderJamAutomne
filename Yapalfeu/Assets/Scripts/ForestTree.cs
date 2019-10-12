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

    State state;

    float stateDuration;

    // Négatif s'il ne brûle pas
    float burning;

    // Start is called before the first frame update
    void Start()
    {
        state = State.SOIL;
        burning = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsBurning())
        {
            burning += Time.deltaTime;

            if (burning >= Config.burnDuration)
                ChangeState(State.BURNT);
        }

        stateDuration += Time.deltaTime;

        switch (state)
        {
            case State.PLANTED_WET:
                if (stateDuration >= Config.plantedGrowDuration)
                    ChangeState(State.YOUNG_DRY);
                break;
            case State.YOUNG_WET:
                if (stateDuration >= Config.youngGrowDuration)
                    ChangeState(State.MATURE);
                break;
        }
    }

    private void ChangeState(State s)
    {
        stateDuration = 0;
        state = s;
    }

    public void Select()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void Deselect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private bool HasSeed()
    {
        return state == State.MATURE && stateDuration >= Config.seedGrowDuration;
    }

    private bool IsBurning()
    {
        return burning >= 0;
    }

    private void StopBurning()
    {
        burning = -1;
    }

    public Action GetAction(Player player)
    {
        if (IsBurning())
        {
            if (player.HasFilledBucket())
                return new Action("Éteindre", Button.A, null, 0, Config.actionDuration, () => { if (player.WaterPlant()) StopBurning(); });
        }
        else
        {
            // TODO : Mettre des combos ou ajuster le temps d'appui
            switch (state)
            {
                case State.SOIL:
                    if (player.HasSeed())
                        return new Action("Planter", Button.A, null, 0, Config.actionDuration, () => { if (player.PlantSeed()) ChangeState(State.PLANTED_DRY); });
                    break;
                case State.PLANTED_DRY:
                    if (player.HasFilledBucket())
                        return new Action("Arroser", Button.A, null, 0, Config.actionDuration, () => { if (player.WaterPlant()) ChangeState(State.PLANTED_WET); });
                    break;
                case State.YOUNG_DRY:
                    if (player.HasFilledBucket())
                        return new Action("Arroser", Button.A, null, 0, Config.actionDuration, () => { if (player.WaterPlant()) ChangeState(State.YOUNG_WET); });
                    break;
                case State.MATURE:
                    if (HasSeed())
                        return new Action("Récolter", Button.A, null, 0, Config.actionDuration, () => { player.HarvestSeed(); stateDuration = 0; });
                    break;
                case State.BURNT:
                    return new Action("Arracher", Button.A, null, 0, Config.actionDuration, () => ChangeState(State.SOIL));
            }
        }

        return null;
    }
}
