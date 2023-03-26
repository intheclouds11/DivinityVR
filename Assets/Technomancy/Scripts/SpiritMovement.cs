using System;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    private HVRPlayerController hvrPlayerController;
    private CharacterController characterController;
    private HVRPlayerInputs playerInputs;
    public Transform hmdTransform;
    public List<GameObject> fadeGOs;
    public bool screenFadeOnCollision;
    private float originalGravity;
    private float originalMaxFallSpeed;
    public bool verticalMovementCameraBased;
    public float SmoothTurnThreshold = .5f;
    public float horizontalSpeed = 2;
    public float verticalSpeed = 2;
    public float forwardSpeed = 2;
    public float sprintSpeedMultipler = 2;
    private bool isSprinting;
    private float previousTurnAmount;


    private void Awake()
    {
        hvrPlayerController = GetComponent<HVRPlayerController>();
        characterController = GetComponent<CharacterController>();
        playerInputs = GetComponent<HVRPlayerInputs>();
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
        if (screenFadeOnCollision)
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
        if (screenFadeOnCollision)
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
        previousTurnAmount = playerInputs.TurnAxis.x;
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

        float clampedX = xMovement;
        float clampedY = yMovement;
        float clampedZ = zMovement;
        // Limit movement when certain distance to physical form
        // if (Mathf.Abs(transform.position.x - transform.parent.GetComponent<LocalUserObjects>().spiritWander.spawnedGOs[0].transform.position.x) > 5)
        // {
        //     clampedX = 0;
        //     clampedX = Mathf.Clamp(xMovement, -100f, 0f);
        // }
        //
        // if (Mathf.Abs(transform.position.y - transform.parent.GetComponent<LocalUserObjects>().spiritWander.spawnedGOs[0].transform.position.y) > 5)
        // {
        //     clampedY = 0;
        //     clampedY = Mathf.Clamp(yMovement, -100f, 0f);
        // }
        //
        // if (Mathf.Abs(transform.position.z - transform.parent.GetComponent<LocalUserObjects>().spiritWander.spawnedGOs[0].transform.position.z) > 5)
        // {
        //     clampedZ = 0;
        //     clampedZ = Mathf.Clamp(zMovement, -100f, 0f);
        // }

        if (isSprinting)
        {
            transform.Translate(clampedX * sprintSpeedMultipler, clampedY * sprintSpeedMultipler, clampedZ * sprintSpeedMultipler);
        }
        else
        {
            transform.Translate(clampedX, clampedY, clampedZ);
        }
    }

    void HandleRotation()
    {
        var input = playerInputs.TurnAxis.x;
        
        if (hvrPlayerController.RotationType == RotationType.Snap)
        {
            if (Math.Abs(input) < hvrPlayerController.SnapThreshold || Mathf.Abs(previousTurnAmount) > hvrPlayerController.SnapThreshold) return;

            var rotation = Quaternion.Euler(0, Mathf.Sign(input) * hvrPlayerController.SnapAmount, 0);
            transform.rotation *= rotation;
        }
        else
        {
            if (Math.Abs(input) < SmoothTurnThreshold) return;

            var rotation = input * hvrPlayerController.SmoothTurnSpeed * Time.deltaTime;
            var rotationVector = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotation, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(rotationVector);
        }
    }
}