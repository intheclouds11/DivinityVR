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
        private float _apNeededForTeleport;
        private ITCTeleporter _teleporter;
        private Vector3 _currentPosition;

        private void OnEnable()
        {
            _playerStats = LocalUserObjects.instance.PlayerStats;
            _playerController = LocalUserObjects.instance.ITCPlayerController;
            _playerController.MovementEnabled = false;
            _teleporter = LocalUserObjects.instance.ITCTeleporter;
            _teleporter.BeforeTeleport.AddListener(BeforeTeleport);
            _teleporter.Dash = true;
            _currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }

        private void OnDisable()
        {
            _playerController.MovementEnabled = true;

            _teleporter.Dash = false;
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
                LocalUserObjects.instance.HUDController.ToggleTeleportCancelReminder(true);
                _apNeededForTeleport = _teleporter.teleportPathLength / apDistanceUnit;
                LocalUserObjects.instance.HUDController.ShowPointerUI(ActionType.Movement, $"AP: {(int) Mathf.Ceil(_apNeededForTeleport)}");

                if (_playerStats.CurrentAP >= _apNeededForTeleport)
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
            _playerStats.UseAP((int) Mathf.Ceil(_apNeededForTeleport));
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

        public void StartTurn()
        {
        }

        public void EndTurn()
        {
        }
    }
}