using HighlightPlus;
using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.HandPoser;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace intheclouds
{
    public class PlayerSwapper : MonoBehaviour
    {
        // public static PlayerSwapper Instance;
        public HVRSocket socket;
        public TextMeshProUGUI selectedPlayerText;
        public float firstNotchDistance = 0.5f;
        public float secondNotchDistance = 1f;
        private PlayerStats _currentControlledPlayer;
        private PlayerStats _selectedPlayer;
        // private HVRGrabbable grabbable;
        private bool _firstNotch;
        private bool _secondNotch;
        private HighlightEffect _highlightEffect;
        private LineRenderer _lineRenderer;

        private void Start()
        {
            // Instance = this;
            // grabbable = GetComponent<HVRGrabbable>();
            _highlightEffect = GetComponent<HighlightEffect>();
            _lineRenderer = GetComponent<LineRenderer>();
            socket.Grabbed.AddListener(OnSwapperReleased);
            socket.Released.AddListener(OnSwapperGrabbed);
            selectedPlayerText.gameObject.SetActive(false);
            _currentControlledPlayer = GameManager.instance.controlledPlayer;
        }

        private void Update()
        {
            if (!socket.IsHoldActive)
            {
                _lineRenderer.enabled = true;
                _lineRenderer.SetPosition(0, socket.transform.position);
                _lineRenderer.SetPosition(1, transform.position);
                selectedPlayerText.gameObject.SetActive(true);
            }
            else
            {
                _lineRenderer.enabled = false;
                selectedPlayerText.gameObject.SetActive(false);
                return;
            }

            // at notches
            if (GameManager.instance.players.Count > 1 && !_secondNotch && Vector3.Distance(transform.position, socket.transform.position) >= secondNotchDistance)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, transform.position);
                _highlightEffect.glowHQColor = Color.yellow;
                _lineRenderer.endColor = Color.yellow;
                _secondNotch = true;
                _selectedPlayer = GameManager.instance.players[1];
                selectedPlayerText.text = _selectedPlayer.Name;
            }
            else if (!_firstNotch && Vector3.Distance(transform.position, socket.transform.position) >= firstNotchDistance)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, transform.position, 0.8f, 1);
                _highlightEffect.glowHQColor = Color.blue;
                _lineRenderer.endColor = Color.blue;
                _firstNotch = true;
                _selectedPlayer = GameManager.instance.players[0];
                selectedPlayerText.text = _selectedPlayer.Name;
            }

            // in between notches
            if (_secondNotch && Vector3.Distance(transform.position, socket.transform.position) < secondNotchDistance)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, transform.position, 0.8f, 1);
                _highlightEffect.glowHQColor = Color.blue;
                _lineRenderer.endColor = Color.blue;
                _secondNotch = false;
            }
            else if (_firstNotch && !_secondNotch && Vector3.Distance(transform.position, socket.transform.position) < firstNotchDistance)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, transform.position, 0.6f, 1);
                _highlightEffect.glowHQColor = Color.white;
                _lineRenderer.endColor = Color.white;
                _firstNotch = false;
            }
        }

        private void OnSwapperReleased(HVRGrabberBase grabber, HVRGrabbable hvrGrabbable)
        {
            if (_selectedPlayer == _currentControlledPlayer)
            {
                Debug.Log("Already controlling selectedPlayer");
            }
            else
            {
                if (_secondNotch)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sparkleSFX, transform.position);
                    Debug.Log($"Swapped to Second notch!");
                    DisableCurrentPlayerObjects();
                    EnableSwappedPlayerObjects(_selectedPlayer);
                    _currentControlledPlayer.PlayerControlled = false;
                    _currentControlledPlayer = _selectedPlayer;
                    _selectedPlayer.PlayerControlled = true;
                }
                else if (_firstNotch)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sparkleSFX, transform.position, 0.8f, 1);
                    Debug.Log($"Swapped to First notch!");
                    DisableCurrentPlayerObjects();
                    EnableSwappedPlayerObjects(_selectedPlayer);
                    _currentControlledPlayer.PlayerControlled = false;
                    _currentControlledPlayer = _selectedPlayer;
                    _selectedPlayer.PlayerControlled = true;
                }
            }

            _firstNotch = false;
            _secondNotch = false;
            _highlightEffect.highlighted = false;
            _highlightEffect.glowHQColor = Color.white;
            _lineRenderer.endColor = Color.white;
        }

        private void OnSwapperGrabbed(HVRGrabberBase grabber, HVRGrabbable hvrGrabbable)
        {
            _highlightEffect.highlighted = true;
        }

        private void DisableCurrentPlayerObjects()
        {
            var currentPlayerObjects = _currentControlledPlayer.GetComponentInParent<LocalUserObjects>();
            currentPlayerObjects.PlayerStats.PlayerControlled = false;
            currentPlayerObjects.ITCPlayerController.enabled = false;
            currentPlayerObjects.HVRPlayerInputs.enabled = false;
            currentPlayerObjects.ITCPlayerInputs.enabled = false;

            currentPlayerObjects.leftController.GetComponentInChildren<HVRGhostHand>().DisplayGhostHand = false;
            currentPlayerObjects.leftController.GetComponent<TrackedPoseDriver>().enabled = false;
            currentPlayerObjects.leftHandModel.GetComponent<HVRHandAnimator>().enabled = false;
            currentPlayerObjects.leftHandGrabber.GetComponent<Rigidbody>().isKinematic = true;
            currentPlayerObjects.leftHandGrabber.GetComponent<HVRJointHand>().Target = currentPlayerObjects.leftHandGrabber.transform;

            currentPlayerObjects.rightController.GetComponentInChildren<HVRGhostHand>().DisplayGhostHand = false;
            currentPlayerObjects.rightController.GetComponent<TrackedPoseDriver>().enabled = false;
            currentPlayerObjects.rightHandModel.GetComponent<HVRHandAnimator>().enabled = false;
            currentPlayerObjects.rightHandGrabber.GetComponent<Rigidbody>().isKinematic = true;
            currentPlayerObjects.rightHandGrabber.GetComponent<HVRJointHand>().Target = currentPlayerObjects.rightHandGrabber.transform;

            var cameraComponents = currentPlayerObjects.Camera.gameObject.GetComponents<Behaviour>();
            foreach (var cameraComponent in cameraComponents)
            {
                cameraComponent.enabled = false;
            }

            // make helmet visible to camera
            foreach (Transform childObject in currentPlayerObjects.Camera.gameObject.transform.GetChild(0))
            {
                if (childObject.CompareTag("Headwear"))
                {
                    childObject.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }

        private void EnableSwappedPlayerObjects(PlayerStats swappedPlayer)
        {
            var swappedPlayerObjects = swappedPlayer.GetComponentInParent<LocalUserObjects>();

            var turnOrderUI = _currentControlledPlayer.GetComponentInParent<LocalUserObjects>().turnOrderUI;
            turnOrderUI.transform.SetParent(swappedPlayerObjects.Camera.transform);
            turnOrderUI.transform.localPosition = Vector3.zero;
            turnOrderUI.transform.localRotation = Quaternion.identity;

            swappedPlayerObjects.PlayerStats.PlayerControlled = true;
            swappedPlayerObjects.ITCPlayerController.enabled = true;
            swappedPlayerObjects.HVRPlayerInputs.enabled = true;
            swappedPlayerObjects.ITCPlayerInputs.enabled = true;

            swappedPlayerObjects.leftController.GetComponentInChildren<HVRGhostHand>().DisplayGhostHand = true;
            swappedPlayerObjects.leftController.GetComponent<TrackedPoseDriver>().enabled = true;
            swappedPlayerObjects.leftHandModel.GetComponent<HVRHandAnimator>().enabled = true;
            swappedPlayerObjects.leftHandGrabber.GetComponent<Rigidbody>().isKinematic = false;
            swappedPlayerObjects.leftHandGrabber.GetComponent<HVRJointHand>().Target =
                swappedPlayerObjects.leftController.GetComponentInChildren<HVRControllerOffset>().transform;

            swappedPlayerObjects.rightController.GetComponentInChildren<HVRGhostHand>().DisplayGhostHand = true;
            swappedPlayerObjects.rightController.GetComponent<TrackedPoseDriver>().enabled = true;
            swappedPlayerObjects.rightHandModel.GetComponent<HVRHandAnimator>().enabled = true;
            swappedPlayerObjects.rightHandGrabber.GetComponent<Rigidbody>().isKinematic = false;
            swappedPlayerObjects.rightHandGrabber.GetComponent<HVRJointHand>().Target =
                swappedPlayerObjects.rightController.GetComponentInChildren<HVRControllerOffset>().transform;

            var cameraGOComponents = swappedPlayerObjects.Camera.gameObject.GetComponents<Behaviour>();
            foreach (var component in cameraGOComponents)
            {
                component.enabled = true;
            }

            // make helmet invisible to camera
            foreach (Transform childObject in swappedPlayerObjects.Camera.gameObject.transform.GetChild(0))
            {
                if (childObject.CompareTag("Headwear"))
                {
                    childObject.gameObject.layer = LayerMask.NameToLayer("InvisibleToMainCamera");
                }
            }

            UserMenu.instance.UserSetup(swappedPlayerObjects.PlayerStats);
        }

        // public void PlayerSwap()
        // {
        //     restart:
        //     foreach (var player in GameManager.Instance.players)
        //     {
        //         // First get currently controlled player
        //         if (currentControlledPlayer == null)
        //         {
        //             if (player.PlayerControlled)
        //             {
        //                 currentControlledPlayer = player;
        //                 goto restart;
        //             }
        //             else
        //             {
        //                 continue;
        //             }
        //         }
        //
        //         // Then if that player is the same as button text, exit coroutine
        //         if (currentControlledPlayer.Name == GetComponentInChildren<TextMeshProUGUI>().text)
        //         {
        //             Debug.Log("Character is already being controlled");
        //             return;
        //         }
        //
        //         if (player.Name == GetComponentInChildren<TextMeshProUGUI>().text)
        //         {
        //             DisableCurrentPlayerObjects();
        //             EnableSwappedPlayerObjects(player);
        //         }
        //     }
        // }
    }
}