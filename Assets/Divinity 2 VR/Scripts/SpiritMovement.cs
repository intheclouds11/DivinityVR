using System;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    private HVRPlayerController hvrPlayerController;
    private HVRPlayerInputs playerInputs;
    public float yMoveSpeed = 2;
    private float originalGravity;
    private float originalMaxFallSpeed;

    void Awake()
    {
        playerInputs = GetComponent<HVRPlayerInputs>();
        hvrPlayerController = GetComponent<HVRPlayerController>();
    }

    private void OnEnable()
    {
        originalGravity = hvrPlayerController.Gravity;
        originalMaxFallSpeed = hvrPlayerController.MaxFallSpeed;
        hvrPlayerController.CanJump = false;
        hvrPlayerController.CanCrouch = false;
        hvrPlayerController.Gravity = 0;
        hvrPlayerController.MaxFallSpeed = 0;
        hvrPlayerController.transform.position = new Vector3(hvrPlayerController.transform.position.x - 0.5f,
            hvrPlayerController.transform.position.y + 0.5f, hvrPlayerController.transform.position.z);
    }

    private void OnDisable()
    {
        hvrPlayerController.CanJump = true;
        hvrPlayerController.CanCrouch = true;
        hvrPlayerController.Gravity = originalGravity;
        hvrPlayerController.MaxFallSpeed = originalMaxFallSpeed;
    }

    void Update()
    {
        var yMovement = playerInputs.RightController.JoystickAxis.y * Time.deltaTime * yMoveSpeed;
        transform.Translate(0, yMovement, 0);
    }
}