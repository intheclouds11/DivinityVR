using System;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    private HVRPlayerController hvrPlayerController;
    private CharacterController characterController;
    public List<GameObject> fadeGOs;
    public bool screenFade;
    private HVRPlayerInputs playerInputs;
    private float originalGravity;
    private float originalMaxFallSpeed;
    public Transform hmdTransform;
    public bool verticalMovementCameraBased;
    public float horizontalSpeed = 2;
    public float verticalSpeed = 2;
    public float forwardSpeed = 2;
    public float sprintSpeed = 2;
    private bool isSprinting;
    private float previousTurnAmount;
    public float snapAmount;

    void Awake()
    {
        playerInputs = GetComponent<HVRPlayerInputs>();
        hvrPlayerController = GetComponent<HVRPlayerController>();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        originalGravity = hvrPlayerController.Gravity;
        originalMaxFallSpeed = hvrPlayerController.MaxFallSpeed;

        hvrPlayerController.CanJump = false;
        hvrPlayerController.CanCrouch = false;
        hvrPlayerController.Gravity = 0;
        hvrPlayerController.MaxFallSpeed = 0;
        characterController.enabled = false;
        if (screenFade)
        {
            fadeGOs[0].GetComponent<HVRCanvasFade>().enabled = false;
        }
        else
        {
            foreach (var fadeGO in fadeGOs)
            {
                fadeGO.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        hvrPlayerController.CanJump = true;
        hvrPlayerController.CanCrouch = true;
        hvrPlayerController.Gravity = originalGravity;
        hvrPlayerController.MaxFallSpeed = originalMaxFallSpeed;
        characterController.enabled = true;
        if (screenFade)
        {
            fadeGOs[0].GetComponent<HVRCanvasFade>().enabled = true;
        }
        else
        {
            foreach (var fadeGO in fadeGOs)
            {
                fadeGO.SetActive(true);
            }
        }
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        previousTurnAmount = playerInputs.RightController.JoystickAxis.x;
    }

    void HandleMovement()
    {
        if (playerInputs.LeftController.JoystickAxis.magnitude > 0.05f)
        {
            if (playerInputs.LeftController.JoystickClicked)
            {
                isSprinting = true;
            }
        }
        else
        {
            isSprinting = false;
        }

        float xMovement = playerInputs.LeftController.JoystickAxis.x * Time.deltaTime * horizontalSpeed;
        float yMovement = 0;
        float zMovement = playerInputs.LeftController.JoystickAxis.y * Time.deltaTime * forwardSpeed;
        if (verticalMovementCameraBased)
        {
            yMovement = playerInputs.LeftController.JoystickAxis.y * Time.deltaTime * hmdTransform.forward.y * verticalSpeed;
        }
        else
        {
            if (playerInputs.RightController.JoystickAxis.y > 0.2f || playerInputs.RightController.JoystickAxis.y < -0.2f)
            {
                yMovement = playerInputs.RightController.JoystickAxis.y * Time.deltaTime * verticalSpeed;
            }
        }

        if (isSprinting)
        {
            transform.Translate(xMovement * sprintSpeed, yMovement * sprintSpeed, zMovement * sprintSpeed);
        }
        else
        {
            transform.Translate(xMovement, yMovement, zMovement);
        }
    }

    void HandleRotation()
    {
        if (Math.Abs(playerInputs.RightController.JoystickAxis.x) < 0.75f || Mathf.Abs(previousTurnAmount) > 0.75f)
            return;

        var rotation = Quaternion.Euler(0, Mathf.Sign(playerInputs.RightController.JoystickAxis.x) * snapAmount, 0);
        transform.rotation *= rotation;


        // if (playerInputs.RightController.JoystickAxis.x > 0.1f)
        // {
        //     characterController.gameObject.transform.Rotate(0, 45, 0);
        // }
        // else if (playerInputs.RightController.JoystickAxis.x < -0.1f)
        // {
        //     characterController.gameObject.transform.Rotate(0, -45, 0);
        //
        // }
    }
}