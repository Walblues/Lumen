using UnityEngine;
using System.Collections.Generic;

public enum ButtonType
{
    UNUSED,
    TOGGLE,
    ON,
    OFF,
    ROTATE,
}

public class VRCubeArrayController : MonoBehaviour
{
    public List<CubeController> cubeLights;

    // We no longer need Awake, OnEnable, or OnDisable for InputActions!

    // Change these to PUBLIC so Unity Events can call them

    public List<int> trigger1Indices;
    public ButtonType trigger1Type;

    public List<int> trigger2Indices;
    public ButtonType trigger2Type;

    public List<int> trigger3Indices;
    public ButtonType trigger3Type;

    public List<int> trigger4Indices;
    public ButtonType trigger4Type;

    public List<int> trigger5Indices;
    public ButtonType trigger5Type;

    private void TriggerButton(List<int> indices, ButtonType type)
    {
        // Handle ROTATE once, not per index
        if (type == ButtonType.ROTATE)
        {
            Rotate();
            return;
        }

        foreach (int index in indices)
        {
            switch (type)
            {
                case ButtonType.TOGGLE:
                    cubeLights[index].toggle();
                    break;

                case ButtonType.ON:
                    cubeLights[index].turnOn();
                    break;

                case ButtonType.OFF:
                    cubeLights[index].turnOff();
                    break;
            }
        }
    }

    public void TriggerButton1()
    {
        TriggerButton(trigger1Indices, trigger1Type);
    }

    public void TriggerButton2()
    {
        TriggerButton(trigger2Indices, trigger2Type);
    }

    public void TriggerButton3()
    {
        TriggerButton(trigger3Indices, trigger3Type);
    }

    public void TriggerButton4()
    {
        TriggerButton(trigger4Indices, trigger4Type);
    }

    public void TriggerButton5()
    {
        TriggerButton(trigger5Indices, trigger5Type);
    }

    public void Rotate()
    {
        bool lastButton = cubeLights[cubeLights.Count - 1].getIsLit();

        for (int i = cubeLights.Count - 1; i > 0; i--)
        {
            cubeLights[i].setIsLit(cubeLights[i - 1].getIsLit());
        }

        cubeLights[0].setIsLit(lastButton);
    }
}