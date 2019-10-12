using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string name;
    public Button button;
    public List<Button> combos;
    public int comboGoal;
    public int actualCombo;
    List<Button> comboBuffer;
    public float pressDuration;
    public float actualDuration;
    public float progression;

    public const float defaultDuration = 0.5f;

    // Action à exécuter
    private System.Action doAction;

    public Action(string name, Button button, List<Button> combos, int comboGoal, System.Action doAction, float pressDuration = defaultDuration)
    {
        this.name = name;
        this.button = button;
        this.combos = combos;
        this.comboGoal = comboGoal;
        this.actualCombo = 0;
        if (this.combos != null)
            this.comboBuffer = new List<Button>(combos);
        this.pressDuration = pressDuration;
        this.actualDuration = 0f;
        this.doAction = doAction;
        this.progression = 0f;
    }

    public bool IsDone()
    {
        return progression >= 1f;
    }

    public void Do()
    {
        if (IsDone())
            return;

        if (combos != null)
        {
            if (InputManager.GetButtonDown(comboBuffer[0]))
                comboBuffer.RemoveAt(0);

            if (comboBuffer.Count == 0)
            {
                comboBuffer.AddRange(combos);
                actualCombo++;
            }

            if (actualCombo == comboGoal)
            {
                doAction();
                progression = 1f;
            }
            else
            {
                progression = ((float)actualCombo + (float)(combos.Count-comboBuffer.Count) /combos.Count) / comboGoal;
            }
        }
        else
        {
            this.actualDuration += Time.deltaTime;
            if (actualDuration >= pressDuration)
            {
                doAction();
                progression = 1f;
            }
            else
            {
                progression = actualDuration / pressDuration;
            }
        }
    }
}
