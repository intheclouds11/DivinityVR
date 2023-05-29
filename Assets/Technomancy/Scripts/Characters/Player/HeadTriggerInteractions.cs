using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class HeadTriggerInteractions : MonoBehaviour
    {
        public float handInTriggerMaxTime = 1f;
        private PlayerStats _playerStats;
        private Potion _potion;
        private float _handInTriggerTime;
        private bool _handInTrigger;
        private ITCSocket _headSocket;

        private void Awake()
        {
            _playerStats = transform.GetComponentInParent<PlayerStats>();
            _headSocket = GetComponent<ITCSocket>();
        }

        private void Update()
        {
            HandInTriggerUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Potion"))
            {
                CheckPotionInteraction(other);
            }
            else if (other.CompareTag("HandTriggerCollider"))
            {
                if (_headSocket.IsGrabbing)
                {
                    _handInTrigger = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("HandTriggerCollider"))
            {
                _headSocket.CanRemoveGrabbable = false;
                _handInTrigger = false;
            }
        }

        private void HandInTriggerUpdate()
        {
            if (_handInTrigger)
            {
                if (_handInTriggerTime >= handInTriggerMaxTime && !_headSocket.CanRemoveGrabbable)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, transform.position, 1, 0.5f, 10, false);
                    _headSocket.CanRemoveGrabbable = true;
                }

                _handInTriggerTime += Time.deltaTime;
            }
            else if (_handInTriggerTime > 0)
            {
                _handInTriggerTime = 0;
            }
        }

        private void CheckPotionInteraction(Collider other)
        {
            _potion = other.transform.parent.GetComponent<Potion>();
            if (_potion.Used || !_potion.Usable || _potion.grabbable.IsSocketed)
            {
                return;
            }

            if (_playerStats.Turn && _playerStats.CurrentAP > _potion.requiredAP)
            {
                _playerStats.UseAP(_potion.requiredAP);
            }
            else if (_playerStats.InCombat)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.errorSFX, transform.position, 1, 0.5f, 10, false);
                return;
            }

            _potion.StartDrinkCoroutine(_playerStats);
        }
    }
}