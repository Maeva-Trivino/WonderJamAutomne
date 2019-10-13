using UnityEngine;

public enum Button
{
    A,
    B,
    X,
    Y,
    LB,
    RB,
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public enum Axis
{
    Horizontal,
    Vertical
}

public class InputManager
{
    public static bool GetButtonDown(Button button)
    {
        switch (button)
        {
            case Button.A:
                return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0);
            case Button.B:
                return Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick2Button1);
            case Button.X:
                return Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Joystick2Button2);
            case Button.Y:
                return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick2Button3);
            case Button.LB:
                return Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick2Button4);
            case Button.RB:
                return Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Joystick2Button5);
            case Button.LEFT:
                return Input.GetAxisRaw("Horizontal") < -.2f;
            case Button.RIGHT:
                return Input.GetAxisRaw("Horizontal") > .2f;
            case Button.UP:
                return Input.GetAxisRaw("Vertical") > .2f;
            case Button.DOWN:
                return Input.GetAxisRaw("Vertical") < -.2f;
        }
        return false;
    }

    public static bool GetButton(Button button)
    {
        switch (button)
        {
            case Button.A:
                return Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Joystick2Button0);
            case Button.B:
                return Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKey(KeyCode.Joystick2Button1);
            case Button.X:
                return Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.Joystick2Button2);
            case Button.Y:
                return Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.Joystick1Button3) || Input.GetKey(KeyCode.Joystick2Button3);
            case Button.LB:
                return Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.Joystick2Button4);
            case Button.RB:
                return Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.Joystick2Button5);
            case Button.LEFT:
                return Input.GetAxisRaw("Horizontal") < -.2f;
            case Button.RIGHT:
                return Input.GetAxisRaw("Horizontal") > .2f;
            case Button.UP:
                return Input.GetAxisRaw("Vertical") > .2f;
            case Button.DOWN:
                return Input.GetAxisRaw("Vertical") < -.2f;
        }
        return false;
    }

    public static float GetAxis(Axis axis)
    {
        switch (axis)
        {
            case Axis.Horizontal:
                return Input.GetAxisRaw("Horizontal");
            case Axis.Vertical : 
                return Input.GetAxisRaw("Vertical");
        };
        return 0f;
    }

    public static string GetButtonName(Button button)
    {
        string[] names = Input.GetJoystickNames();
        if (names.Length > 0 && names[0].Length > 0)
        {
            switch (button)
            {
                case Button.A:
                    return "A";
                case Button.B:
                    return "B";
                case Button.LEFT:
                    return "←";
                case Button.RIGHT:
                    return "→";
                case Button.UP:
                    return "↑";
                case Button.DOWN:
                    return "↓";
            }
        }
        else
        {
            switch (button)
            {
                case Button.A:
                    return "E";
                case Button.B:
                    return "F";
                case Button.LEFT:
                    return "←";
                case Button.RIGHT:
                    return "→";
                case Button.UP:
                    return "↑";
                case Button.DOWN:
                    return "↓";
            }
        }

        return "Unknown key name";
    }
}