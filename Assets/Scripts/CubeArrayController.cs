using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CubeArrayController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<CubeController> cubeLights;

    private InputAction toggleAction1;
    private InputAction toggleAction2;
    private InputAction toggleAction3;

    void Awake()
    {
        toggleAction1 = new InputAction(
            name: "Toggle1",
            type: InputActionType.Button,
            binding: "<Keyboard>/1"
        );

        toggleAction1.performed += Toggle1;

        toggleAction2 = new InputAction(
            name: "Toggle2",
            type: InputActionType.Button,
            binding: "<Keyboard>/2"
        );

        toggleAction2.performed += Toggle2;

        toggleAction3 = new InputAction(
            name: "Toggle3",
            type: InputActionType.Button,
            binding: "<Keyboard>/3"
        );

        toggleAction3.performed += Toggle3;

    }
    void OnEnable()
    {
        toggleAction1.Enable();
        toggleAction2.Enable();
        toggleAction3.Enable();
    }

    void OnDisable()
    {
        toggleAction1.Disable();
        toggleAction2.Disable();
        toggleAction3.Disable();
    }

    void Start()
    {

    }

    // Update is called once per frame
    public void Rotate()
    {
        bool lastButton = cubeLights[cubeLights.Count - 1].getIsLit();

        for (int i = cubeLights.Count - 1; i > 0; i--)
        {
            cubeLights[i].setIsLit(cubeLights[i - 1].getIsLit());
        }

        cubeLights[0].setIsLit(lastButton);
    }

    private void Toggle1(InputAction.CallbackContext context)
    {
        for (int i = 0; i < 5; i++)
        {
            cubeLights[i].toggle();
        }
    }

    private void Toggle2(InputAction.CallbackContext context)
    {
        for (int i = 2; i < 6; i++)
        {
            cubeLights[i].toggle();
        }
    }

    private void Toggle3(InputAction.CallbackContext context)
    {
        Rotate();
    }
}
