using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEngine;

public class PlayerInputsManager : MonoBehaviour
{
    private HVRPlayerInputs playerInputs;
    public UserMenu userMenu;

    void Start()
    {
        playerInputs = GetComponent<HVRPlayerInputs>();
    }

    void Update()
    {
        // UserMenu toggle
        if (playerInputs.LeftController.SecondaryButtonState.JustActivated)
        {
            if (userMenu.gameObject.activeInHierarchy && !userMenu.followPlayer)
            {
                // userMenu.transform.SetParent(userMenu.originalParent, false);
                // userMenu.transform.localPosition = userMenu.originalLocalPosition;
                // userMenu.transform.localRotation = userMenu.originalLocalRotation;
            }

            userMenu.gameObject.SetActive(!userMenu.gameObject.activeInHierarchy);
        }
    }
}