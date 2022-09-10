using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class MagicSelector : MonoBehaviour
    {
        public PlayerStats player;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Transform spawnParent;
        private GameObject[] magicSlots = new GameObject[5];
        public GameObject selectedMagic;
        public float deselectCooldown = 1.5f;
        public float deselectCooldownTimer;

        private void Awake()
        {
            player = transform.root.GetComponent<PlayerStats>();
            spawnPosition = transform.localPosition;
            spawnRotation = transform.localRotation;
            spawnParent = transform.parent;
            for (int i = 0; i < magicSlots.Length; i++)
            {
                magicSlots[i] = transform.GetChild(i).gameObject;
            }
        }

        void Update()
        {
            CheckTouchPadPressed();
        }


        private void CheckTouchPadPressed()
        {
            if (player.LocalUserObjects.HVRPlayerInputs.LeftController.TrackpadButtonState.JustActivated)
            {
                Activate();
            }

            else if (player.LocalUserObjects.HVRPlayerInputs.LeftController.TrackpadButtonState.JustDeactivated)
            {
                Deactivate();
            }
        }

        public void Activate()
        {
            transform.parent = transform.root;
            var newEulerAngles = transform.eulerAngles;
            newEulerAngles = new Vector3(0, newEulerAngles.y, 0);
            transform.eulerAngles = newEulerAngles;

            foreach (var magicSlot in magicSlots)
            {
                magicSlot.SetActive(true);
            }
            
            player.LocalUserObjects.handAugmentHighlight.highlighted = true;
        }

        public void Deactivate()
        {
            if (!selectedMagic)
            {
                player.LocalUserObjects.handAugmentHighlight.overlayColor = player.statsSO.baseHandAugmentColor;
                player.LocalUserObjects.handAugmentHighlight.SetGlowColor(player.statsSO.baseHandAugmentColor);
                player.LocalUserObjects.handAugmentHighlight.highlighted = false;
            }

            transform.parent = spawnParent;
            transform.localPosition = spawnPosition;
            transform.localRotation = spawnRotation;
            foreach (var magicSlot in magicSlots)
            {
                magicSlot.SetActive(false);
            }
        }
        // private void CheckTouchPadTouched()
        // {
        //     if (selectedMagic)
        //     {
        //         if (player.LocalUserObjects.HVRPlayerInputs.LeftController.TrackPadTouchState.JustActivated)
        //         {
        //             deselectCooldownTimer += 1;
        //
        //             if (deselectCooldownTimer > deselectCooldown)
        //             {
        //                 selectedMagic.SetActive(false);
        //                 selectedMagic = null;
        //                 deselectCooldownTimer = 0;
        //             }
        //         }
        //         else
        //         {
        //             if (deselectCooldownTimer >= 0)
        //             {
        //                 deselectCooldownTimer -= Time.deltaTime;
        //             }
        //         }
        //     }
        // }
    }
}