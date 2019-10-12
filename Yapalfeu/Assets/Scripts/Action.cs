using System.Collections.Generic;

public class Action
{
    public string name;
    public Button button;
    public List<Button> combos;
    public int comboGoal;
    public float pressDuration;

    public Action(string name, Button button, List<Button> combos, int comboGoal, float pressDuration)
    {
        this.name = name;
        this.button = button;
        this.combos = combos;
        this.comboGoal = comboGoal;
        this.pressDuration = pressDuration;
    }
}
