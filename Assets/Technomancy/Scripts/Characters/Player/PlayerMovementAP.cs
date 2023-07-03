using HurricaneVR.Framework.Core.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        public float playerLeanThreshold = 1;
        public float apDistanceUnit = 4f;
        public GameObject lastValidPositionMarker;

        private HVRPlayerController _playerController;
        private PlayerStats _playerStats;
        public int apRequiredRounded { get; private set; }
        private ITCTeleporter _teleporter;
        private Vector3 _currentPosition;
        private float apRequiredNonRounded;
        private float apRemainder;
        private bool firstTeleport;

        private void OnEnable()
        {
            _playerStats = LocalUserObjects.instance.PlayerStats;
            _playerController = LocalUserObjects.instance.ITCPlayerController;
            _playerController.MovementEnabled = false;
            _teleporter = LocalUserObjects.instance.ITCTeleporter;
            _teleporter.BeforeTeleport.AddListener(BeforeTeleport);
            // _teleporter.Dash = true;
            _currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
            apRemainder = 0;
            firstTeleport = true;
        }

        private void OnDisable()
        {
            _teleporter.UpdateTeleporterColor(default);
            _playerController.MovementEnabled = true;
            // _teleporter.Dash = false;
            _teleporter.BeforeTeleport.RemoveListener(BeforeTeleport);
        }

        private void Update()
        {
            CheckLean();
            CheckTeleport();

            if (_teleporter.TeleportState == TeleportState.Dashing)
            {
                _currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }

        private void CheckTeleport()
        {
            if (!_playerStats.CanPerformActions()) return;

            if (_teleporter.IsAiming)
            {
                if (firstTeleport)
                {
                    apRequiredNonRounded = _teleporter.teleportPathLength / apDistanceUnit;
                    apRequiredRounded = Mathf.CeilToInt(apRequiredNonRounded);

                }
                else
                {
                    apRequiredNonRounded = _teleporter.teleportPathLength / apDistanceUnit - apRemainder;
                    Debug.Log($"after first teleport apRequiredNonRounded: {apRequiredNonRounded}");
                    if (apRequiredNonRounded <= 0)
                    {
                        apRequiredRounded = 0;
                    }
                    else
                    {
                        apRequiredRounded = Mathf.CeilToInt(apRequiredNonRounded);
                    }
                }
                
                // Debug.Log($"1 - _teleporter.teleportPathLength % apDistanceUnit / apDistanceUnit: {1 - _teleporter.teleportPathLength % apDistanceUnit / apDistanceUnit}");

                LocalUserObjects.instance.HUDController.ToggleTeleportCancelReminder(true);
                LocalUserObjects.instance.HUDController.ShowPointerUI(ActionType.Movement, $"AP: {apRequiredRounded}");

                _teleporter.UpdateTeleporterColor(apRequiredRounded);

                if (_playerStats.CurrentAP >= apRequiredRounded)
                {
                    _teleporter.playerHasEnoughAP = true;
                }
                else
                {
                    _teleporter.playerHasEnoughAP = false;
                }
            }
        }

        private void BeforeTeleport(Vector3 arg0)
        {
            firstTeleport = false;
            apRemainder = 1 - _teleporter.teleportPathLength % apDistanceUnit / apDistanceUnit;
            _playerStats.UseAP(apRequiredRounded);
        }

        private void CheckLean()
        {
            var distance = Vector3.Distance(_playerController.transform.position, _currentPosition);

            if (distance > playerLeanThreshold)
            {
                if (_playerStats.Leaning) return;
                LocalUserObjects.instance.HUDController.ToggleLeanWarning(true);
                LocalUserObjects.instance.HVRPlayerInputs.UpdateInputs = false;
                _playerStats.Leaning = true;
                lastValidPositionMarker.SetActive(true);
                lastValidPositionMarker.transform.position = _currentPosition;
            }
            else
            {
                if (!_playerStats.Leaning) return;
                LocalUserObjects.instance.HUDController.ToggleLeanWarning(false);
                LocalUserObjects.instance.HVRPlayerInputs.UpdateInputs = true;
                _playerStats.Leaning = false;
                lastValidPositionMarker.SetActive(false);
            }
        }

        public void ResetCurrentPosition()
        {
            _currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }

        public void StartTurn()
        {
        }

        public void EndTurn()
        {
        }
    }
}