using UnityEngine;
using System.Collections.Generic;

public class VRCubeArrayController : MonoBehaviour
{
    public List<CubeController> cubeLights;

    // We no longer need Awake, OnEnable, or OnDisable for InputActions!

    // Change these to PUBLIC so Unity Events can call them
    public void TriggerButton1()
    {
        for (int i = 0; i < 5; i++)
        {
            cubeLights[i].toggle();
        }
    }

    public void TriggerButton2()
    {
        for (int i = 2; i < 6; i++)
        {
            cubeLights[i].toggle();
        }
    }

    public void TriggerButton3()
    {
        Rotate();
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