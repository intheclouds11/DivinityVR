using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public enum ActionType
    {
        Attack,
        Movement,
        Heal
    }
    public class GenericPointerInfo : MonoBehaviour
    {
        public TextMeshProUGUI InfoText;
        public GameObject AttackIcon;
        public GameObject MovementIcon;

        private void Awake()
        {
            LocalUserObjects.Instance.genericPointerInfo.gameObject.SetActive(false);
        }

        public void ShowInfo(ActionType type, string text)
        {
            //todo: add heal check. Highest activation priority
            // Movement hud takes priority
            if (type == ActionType.Attack && !LocalUserObjects.Instance.genericPointerInfo.MovementIcon.activeSelf)
            {
                LocalUserObjects.Instance.genericPointerInfo.AttackIcon.SetActive(true);
                LocalUserObjects.Instance.genericPointerInfo.InfoText.text = text;
            }
            else if (type == ActionType.Movement)
            {
                LocalUserObjects.Instance.genericPointerInfo.AttackIcon.SetActive(false);
                LocalUserObjects.Instance.genericPointerInfo.MovementIcon.SetActive(true);
                LocalUserObjects.Instance.genericPointerInfo.InfoText.text = text;
            }
        
            LocalUserObjects.Instance.genericPointerInfo.gameObject.SetActive(true);
        }

        public void HideInfo(ActionType infoType)
        {
            if (infoType == ActionType.Attack)
            {
                LocalUserObjects.Instance.genericPointerInfo.AttackIcon.SetActive(false);
            }
            else if (infoType == ActionType.Movement)
            {
                LocalUserObjects.Instance.genericPointerInfo.MovementIcon.SetActive(false);
            }

            if (!LocalUserObjects.Instance.genericPointerInfo.AttackIcon.activeSelf && !LocalUserObjects.Instance.genericPointerInfo.MovementIcon.activeSelf)
            {
                LocalUserObjects.Instance.genericPointerInfo.gameObject.SetActive(false);
            }
        }
    }
}
