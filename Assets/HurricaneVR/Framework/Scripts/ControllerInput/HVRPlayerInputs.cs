using System;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Shared;
using UnityEngine;
using UnityEngine.Serialization;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HurricaneVR.Framework.ControllerInput
{
    /// <summary>
    /// Used by the player controller, teleporter, and hand grabbing systems to drive their actions.
    /// Subclass and override the virtual methods if you wish to customize inputs per device.
    /// </summary>
    public class HVRPlayerInputs : MonoBehaviour
    {
        public bool AllowTeleportInput = false;
        
        [Header("Grab Settings")]
        public bool CanDistanceGrab = true;
        public bool CanTriggerGrab;
        public bool CanTriggerRelease = false;

        [Tooltip("For non flick style force grabber")]
        public HVRForceGrabActivation NonFlickForceGrabActivation = HVRForceGrabActivation.Grip;

        [Range(0f, 1f)]
        public float TriggerGrabThreshold = .7f;

        [Header("Inputs Debugging")]
        public Vector2 MovementAxis;
        public Vector2 TurnAxis;

        public bool IsTeleportActivated;
        public bool IsTeleportDeactivated;

        public bool IsSprintingActivated;
        public bool SprintRequiresDoubleClick;

        public bool IsCrouchActivated;
        public bool IsStandActivated;

        public HVRButtonState JumpState;
        public HVRButtonState CrouchState;
        public HVRButtonState StandState;

        public HVRButtonState LeftTriggerGrabState;
        public HVRButtonState RightTriggerGrabState;
        
        public bool IsLeftHoldActive;
        public bool IsLeftTriggerHoldActive;
        
        public bool IsRightHoldActive;
        public bool IsRightTriggerHoldActive;

        public bool IsLeftForceGrabActive;
        public bool IsRightForceGrabActive;

        public bool IsLeftForceGrabActivated;
        public bool IsRightForceGrabActivated;
        
        public bool isLeftAbilitySelectorActive;
        public bool isRightAbilitySelectorActive;

        public bool IsJumpActivated;

        public HVRHandSide TeleportHandSide = HVRHandSide.Right;
        public bool SwapMovementAxis;


        [Header("Debugging")]
        public bool UseWASD;

        public bool IsMouseDown;
        public Vector2 MouseAxis;

        public bool UpdateInputs { get; set; } = true;

        public HVRController RightController => HVRInputManager.Instance.RightController;
        public HVRController LeftController => HVRInputManager.Instance.LeftController;

        public HVRControllerType RightControllerType => RightController.ControllerType;
        public HVRControllerType LeftControllerType => LeftController.ControllerType;

        public HVRController TeleportController => TeleportHandSide == HVRHandSide.Left ? HVRInputManager.Instance.LeftController : HVRInputManager.Instance.RightController;

        public void Update()
        {
            UseWASD = HVRManager.Instance.debugMode && !HVRManager.Instance.isDesktopMode;
            UpdateInput();
            AfterInputUpdate();
        }

        protected virtual void UpdateInput()
        {
            if (!UpdateInputs)
                return;

            ResetState(ref LeftTriggerGrabState);
            ResetState(ref RightTriggerGrabState);
            SetState(ref LeftTriggerGrabState, LeftController.Trigger > TriggerGrabThreshold);
            SetState(ref RightTriggerGrabState, RightController.Trigger > TriggerGrabThreshold);

            MovementAxis = GetMovementAxis();
            TurnAxis = GetTurnAxis();
            IsTeleportActivated = GetTeleportActivated();
            IsTeleportDeactivated = GetTeleportDeactivated();
            IsSprintingActivated = GetSprinting();

            IsCrouchActivated = GetCrouch();
            
            IsLeftHoldActive = GetIsLeftHoldActive();
            IsRightHoldActive = GetIsRightHoldActive();

            isLeftAbilitySelectorActive = GetIsLeftAbilitySelectorInputActive();
            isRightAbilitySelectorActive = GetIsRightAbilitySelectorInputActive();

            GetForceGrabActivated(out IsLeftForceGrabActivated, out IsRightForceGrabActivated);
            GetForceGrabActive(out IsLeftForceGrabActive, out IsRightForceGrabActive);

            IsJumpActivated = GetIsJumpActivated();
            IsStandActivated = GetStand();
            MouseAxis = GetMouse(out IsMouseDown);

            ResetState(ref CrouchState);
            ResetState(ref StandState);
            ResetState(ref JumpState);

            SetState(ref CrouchState, IsCrouchActivated);
            SetState(ref StandState, IsStandActivated);
            SetState(ref JumpState, IsJumpActivated);
        }

        protected virtual void AfterInputUpdate()
        {

        }



        protected void ResetState(ref HVRButtonState buttonState)
        {
            buttonState.JustDeactivated = false;
            buttonState.JustActivated = false;
            buttonState.Value = 0f;
        }

        protected void SetState(ref HVRButtonState buttonState, bool pressed)
        {
            if (pressed)
            {
                if (!buttonState.Active)
                {
                    buttonState.JustActivated = true;
                    buttonState.Active = true;
                }
            }
            else
            {
                if (buttonState.Active)
                {
                    buttonState.Active = false;
                    buttonState.JustDeactivated = true;
                }
            }
        }


        protected virtual bool GetStand()
        {
            return false;
        }

        protected virtual bool GetIsJumpActivated()
        {
            // if (RightController.ControllerType == HVRControllerType.Vive)
            // {
            //     return false;//todo
            // }

            if (RightController.PrimaryButtonState.Active)
            {
                return true;
            }
            return false;
        }

        protected virtual void GetForceGrabActivated(out bool left, out bool right)
        {
            if (!CanDistanceGrab)
            {
                left = false;
                right = false;
                return;
            }

            if (NonFlickForceGrabActivation == HVRForceGrabActivation.Grip)
            {
                left = LeftController.GripButtonState.JustActivated;
                right = RightController.GripButtonState.JustActivated;
            }
            else
            {
                left = LeftController.GripButtonState.Active && LeftController.TriggerButtonState.JustActivated || LeftController.TriggerButtonState.Active && LeftController.GripButtonState.JustActivated;
                right = RightController.GripButtonState.Active && RightController.TriggerButtonState.JustActivated || RightController.TriggerButtonState.Active && RightController.GripButtonState.JustActivated;
            }
        }

        protected virtual void GetForceGrabActive(out bool left, out bool right)
        {
            if (!CanDistanceGrab)
            {
                left = false;
                right = false;
                return;
            }

            left = LeftController.GripButtonState.Active;
            right = RightController.GripButtonState.Active;
        }

        public bool IsAbilitySelectorActive(HVRHandSide side)
        {
            return side == HVRHandSide.Left ? isLeftAbilitySelectorActive : isRightAbilitySelectorActive;
        }

        public bool GetForceGrabActivated(HVRHandSide side)
        {
            if (!CanDistanceGrab)
            {
                return false;
            }

            return side == HVRHandSide.Left ? IsLeftForceGrabActivated : IsRightForceGrabActivated;
        }

        public bool GetForceGrabActive(HVRHandSide side)
        {
            if (!CanDistanceGrab)
            {
                return false;
            }

            return side == HVRHandSide.Left ? IsLeftForceGrabActive : IsRightForceGrabActive;
        }

        public bool GetHoldActive(HVRHandSide side)
        {
            return side == HVRHandSide.Left ? IsLeftHoldActive : IsRightHoldActive;
        }

        public HVRButtonState GetTriggerGrabState(HVRHandSide side)
        {
            return side == HVRHandSide.Left ? LeftTriggerGrabState : RightTriggerGrabState;
        }
        
        public HVRButtonState GetGripState(HVRHandSide side)
        {
            return side == HVRHandSide.Left ? LeftController.GripButtonState : RightController.GripButtonState;
        }

        protected virtual bool GetIsLeftHoldActive()
        {
            IsLeftTriggerHoldActive = LeftController.Trigger > TriggerGrabThreshold;
            if (CanTriggerGrab && IsLeftTriggerHoldActive)
            {
                return true;
            }
            return LeftController.GripButtonState.Active;
        }

        protected virtual bool GetIsRightHoldActive()
        {
            IsRightTriggerHoldActive = RightController.Trigger > TriggerGrabThreshold;
            if (CanTriggerGrab && IsRightTriggerHoldActive)
            {
                return true;
            }
            return RightController.GripButtonState.Active;
        }
        
        protected virtual bool GetIsLeftAbilitySelectorInputActive()
        {
            if (LeftControllerType == HVRControllerType.Knuckles)
            {
                return LeftController.IndexTrackpadButtonState.Active;
            }

            return LeftController.PrimaryButtonState.Active && LeftController.SecondaryButtonState.Active;
        }
        
        protected virtual bool GetIsRightAbilitySelectorInputActive()
        {
            if (RightControllerType == HVRControllerType.Knuckles)
            {
                return RightController.IndexTrackpadButtonState.Active;
            }

            return RightController.PrimaryButtonState.Active && RightController.SecondaryButtonState.Active;
        }

        protected virtual Vector2 GetMovementAxis()
        {
            if (UseWASD)
            {
                var wasd = CheckWASD();
                if (wasd.sqrMagnitude > 0f)
                    return wasd;
            }

            if (SwapMovementAxis)
            {
                if (RightController.ControllerType == HVRControllerType.Vive)
                {
                    if (RightController.TrackpadButtonState.Active)
                        return RightController.TrackpadAxis;
                    return Vector2.zero;
                }

                return RightController.JoystickAxis;
            }

            if (LeftController.ControllerType == HVRControllerType.Vive)
            {
                if (LeftController.TrackpadButtonState.Active)
                    return LeftController.TrackpadAxis;
                return Vector2.zero;
            }

            return LeftController.JoystickAxis;
        }

        private Vector2 CheckWASD()
        {
            var x = 0f;
            var y = 0f;

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.W))
                y += 1f;
            if (Input.GetKey(KeyCode.S))
                y -= 1f;
            if (Input.GetKey(KeyCode.A))
                x += -1f;
            if (Input.GetKey(KeyCode.D))
                x += 1f;
#elif ENABLE_INPUT_SYSTEM
            if (Keyboard.current[Key.W].isPressed)
                y += 1f;
            if (Keyboard.current[Key.S].isPressed)
                y -= 1f;
            if (Keyboard.current[Key.A].isPressed)
                x += -1f;
            if (Keyboard.current[Key.D].isPressed)
                x += 1f;
#endif

            return new Vector2(x, y);
        }

        protected virtual Vector2 GetTurnAxis()
        {
            if (SwapMovementAxis)
            {
                if (LeftController.ControllerType == HVRControllerType.Vive)
                {
                    if (Mathf.Abs(LeftController.TrackpadAxis.y) > .6f)
                        return Vector2.zero;

                    if (LeftController.TrackpadButtonState.Active)
                    {
                        return LeftController.TrackpadAxis;
                    }
                    return Vector2.zero;
                }

                return LeftController.JoystickAxis;
            }

            if (RightController.ControllerType == HVRControllerType.Vive)
            {
                if (Mathf.Abs(RightController.TrackpadAxis.y) > .6f)
                    return Vector2.zero;

                if (RightController.TrackpadButtonState.Active)
                {

                    return RightController.TrackpadAxis;
                }
                return Vector2.zero;
            }

            return RightController.JoystickAxis;
        }

        protected virtual bool GetTeleportDeactivated()
        {
            if (HVRInputManager.Instance.RightController.ControllerType == HVRControllerType.Vive)
            {
                return HVRController.GetButtonState(HVRHandSide.Right, HVRButtons.Menu).JustDeactivated;
            }

            return TeleportController.JoystickAxis.y > -.25f;
        }

        protected virtual bool GetTeleportActivated()
        {
            if (!AllowTeleportInput)
            {
                return false;
            }
            if (HVRInputManager.Instance.RightController.ControllerType == HVRControllerType.Vive)
            {
                return HVRController.GetButtonState(HVRHandSide.Right, HVRButtons.Menu).Active;
            }
            
            return TeleportController.JoystickAxis.y < -.5f && Mathf.Abs(TeleportController.JoystickAxis.x) < .75f;
        }

        protected virtual bool GetSprinting()
        {
            if (LeftController.ControllerType == HVRControllerType.Vive)
            {
                SprintRequiresDoubleClick = true;
                return LeftController.TrackpadButtonState.JustActivated;
            }

            SprintRequiresDoubleClick = false;
            if (RightController.ControllerType == HVRControllerType.WMR)
            {
                return RightController.TrackPadRight.JustActivated;
            }

            //controls that allow you to depress the joystick (wmr opens up steamvr)
            return LeftController.JoystickButtonState.JustActivated;
        }

        protected virtual bool GetCrouch()
        {
            if (RightController.ControllerType == HVRControllerType.Vive)
            {
                return RightController.TrackPadUp.JustActivated;
            }

            if (RightController.ControllerType == HVRControllerType.WMR)
            {
                return RightController.TrackPadDown.JustActivated;
            }

            return RightController.SecondaryButtonState.JustActivated;
        }

        protected virtual Vector2 GetMouse(out bool mouseDown)
        {
            mouseDown = false;

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButton(1))
            {
                mouseDown = true;
                return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
#elif ENABLE_INPUT_SYSTEM
            if (Mouse.current.rightButton.isPressed)
            {
                mouseDown = true;
                return Mouse.current.delta.ReadValue();
            }
#endif

            return Vector2.zero;
        }
    }

    public enum HVRForceGrabActivation
    {
        Grip,
        GripHoldTriggerPress
    }
}