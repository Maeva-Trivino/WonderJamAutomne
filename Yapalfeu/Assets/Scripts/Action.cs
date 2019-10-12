using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string name;
    public Button button;
    public List<Button> combos;
    public int comboGoal;
    public float pressDuration;
    public const float defaultDuration = 0.5f;

    // Action à exécuter
    private System.Action doAction;

    public Action(string name, Button button, List<Button> combos, int comboGoal, System.Action doAction, float pressDuration = defaultDuration)
    {
        this.name = name;
        this.button = button;
        this.combos = combos;
        this.comboGoal = comboGoal;
        this.pressDuration = pressDuration;
        this.doAction = doAction;
    }

    public void Do()
    {
        doAction();
    }

    public IEnumerator ListenCombo()
    {
        List<Button> bufferCombos = new List<Button>(combos);
        while(comboGoal > 0 && InputManager.GetButtonDown(Button.A))
        {
            if(InputManager.GetButtonDown(bufferCombos[0]))
                bufferCombos.RemoveAt(0);

            if (bufferCombos.Count == 0)
            {
                bufferCombos.AddRange(combos);
                comboGoal--;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
