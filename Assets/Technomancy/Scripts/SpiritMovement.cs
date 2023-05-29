using System;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    private HVRPlayerController _hvrPlayerController;
    private CharacterController _characterController;
    private HVRPlayerInputs _playerInputs;
    public Transform hmdTransform;
    public List<GameObject> fadeGOs;
    public bool screenFadeOnCollision;
    private float _originalGravity;
    private float _originalMaxFallSpeed;
    public bool verticalMovementCameraBased;
    public float SmoothTurnThreshold = .5f;
    public float horizontalSpeed = 2;
    public float verticalSpeed = 2;
    public float forwardSpeed = 2;
    public float sprintSpeedMultipler = 2;
    private bool _isSprinting;
    private float _previousTurnAmount;


    private void Awake()
    {
        _hvrPlayerController = GetComponent<HVRPlayerController>();
        _characterController = GetComponent<CharacterController>();
        _playerInputs = GetComponent<HVRPlayerInputs>();
    }

    private void OnEnable()
    {
        _originalGravity = _hvrPlayerController.Gravity;
        _originalMaxFallSpeed = _hvrPlayerController.MaxFallSpeed;

        _hvrPlayerController.CanJump = false;
        _hvrPlayerController.CanCrouch = false;
        _hvrPlayerController.Gravity = 0;
        _hvrPlayerController.MaxFallSpeed = 0;
        _characterController.enabled = false;
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
        _hvrPlayerController.CanJump = true;
        _hvrPlayerController.CanCrouch = true;
        _hvrPlayerController.Gravity = _originalGravity;
        _hvrPlayerController.MaxFallSpeed = _originalMaxFallSpeed;
        _characterController.enabled = true;
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
        _previousTurnAmount = _playerInputs.TurnAxis.x;
    }

    void HandleMovement()
    {
        if (_playerInputs.LeftController.JoystickAxis.magnitude > 0.05f)
        {
            if (_playerInputs.LeftController.JoystickClicked)
            {
                _isSprinting = true;
            }
        }
        else
        {
            _isSprinting = false;
        }

        float xMovement = _playerInputs.LeftController.JoystickAxis.x * Time.deltaTime * horizontalSpeed;
        float yMovement = 0;
        float zMovement = _playerInputs.LeftController.JoystickAxis.y * Time.deltaTime * forwardSpeed;
        if (verticalMovementCameraBased)
        {
            yMovement = _playerInputs.LeftController.JoystickAxis.y * Time.deltaTime * hmdTransform.forward.y * verticalSpeed;
        }
        else
        {
            if (_playerInputs.RightController.JoystickAxis.y > 0.2f || _playerInputs.RightController.JoystickAxis.y < -0.2f)
            {
                yMovement = _playerInputs.RightController.JoystickAxis.y * Time.deltaTime * verticalSpeed;
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

        if (_isSprinting)
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
        var input = _playerInputs.TurnAxis.x;
        
        if (_hvrPlayerController.RotationType == RotationType.Snap)
        {
            if (Math.Abs(input) < _hvrPlayerController.SnapThreshold || Mathf.Abs(_previousTurnAmount) > _hvrPlayerController.SnapThreshold) return;

            var rotation = Quaternion.Euler(0, Mathf.Sign(input) * _hvrPlayerController.SnapAmount, 0);
            transform.rotation *= rotation;
        }
        else
        {
            if (Math.Abs(input) < SmoothTurnThreshold) return;

            var rotation = input * _hvrPlayerController.SmoothTurnSpeed * Time.deltaTime;
            var rotationVector = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotation, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(rotationVector);
        }
    }
}