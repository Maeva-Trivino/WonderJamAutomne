using System.Collections.Generic;

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
}
