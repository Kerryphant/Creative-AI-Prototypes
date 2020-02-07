using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarUserControl : MonoBehaviour
{
    private CarController carController;
    public InputMaster controls;

    float horizontal;
    float accelerate;
    float brake;
    bool handbrake;

    void Awake()
    {
        carController = GetComponent<CarController>();

        controls = new InputMaster();

        controls.CarControls.Turning.performed += ctx => horizontal = ctx.ReadValue<float>();
        controls.CarControls.Turning.canceled += ctx => horizontal = 0.0f;

        controls.CarControls.Accelerate.performed += ctx => accelerate = ctx.ReadValue<float>();
        controls.CarControls.Accelerate.canceled += ctx => accelerate = 0.0f;

        controls.CarControls.Brake.performed += ctx => brake = ctx.ReadValue<float>();
        controls.CarControls.Brake.canceled += ctx => brake = 0.0f;

        controls.CarControls.Handbrake.performed += ctx => handbrake = true;
        controls.CarControls.Handbrake.canceled += ctx => handbrake = false;
    }

    private void FixedUpdate()
    {
        Debug.Log("Horizontal: " + horizontal + " accelerate: " + accelerate + " brake: " + brake + " handbrake: " + handbrake);

        carController.Move(horizontal, accelerate, brake, handbrake);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
