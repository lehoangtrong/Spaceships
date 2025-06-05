using UnityEngine;

public class InputSettings : MonoBehaviour
{
    public static ControlType CurrentControl = ControlType.Keyboard;

    public void SetControlType(int index)
    {
        if (index == 0)
            CurrentControl = ControlType.Keyboard;
        else if (index == 1)
            CurrentControl = ControlType.Mouse;
    }
}
