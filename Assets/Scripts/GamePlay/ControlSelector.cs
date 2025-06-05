using UnityEngine;
using UnityEngine.UI;

public class ControlSelector : MonoBehaviour
{
    public Dropdown controlDropdown;

    void Start()
    {
        controlDropdown.onValueChanged.AddListener(OnControlChanged);
        controlDropdown.value = (int)InputSettings.CurrentControl;
    }

    void OnControlChanged(int index)
    {
        InputSettings.CurrentControl = (ControlType)index;
        Debug.Log("Đã chọn điều khiển: " + InputSettings.CurrentControl);
    }
}
