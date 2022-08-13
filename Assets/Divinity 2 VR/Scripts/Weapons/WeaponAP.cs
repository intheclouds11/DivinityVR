using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponAP : MonoBehaviour
{
    public PlayerStats wieldingUser;
    public float requiredHitSpeed = 1f;
    public int requiredHitAP = 2;
    public bool inCombat;
    public int baseDamage = 10;
    private Rigidbody rb;
    private HVRGrabbable grabbable;
    private float hitCooldown;

    private void Awake()
    {
        grabbable = GetComponent<HVRGrabbable>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!inCombat) return;

        if (hitCooldown > 0)
        {
            hitCooldown -= Time.deltaTime;
        }
    }

    public void StartTurnSetup()
    {
        inCombat = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!inCombat || hitCooldown > 0) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rb.velocity.magnitude > requiredHitSpeed)
            {
                if (wieldingUser.currentAP > requiredHitAP)
                {
                    collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(baseDamage);
                    wieldingUser.currentAP -= requiredHitAP;
                    hitCooldown += 2f;
                    if (wieldingUser.currentAP < requiredHitAP)
                    {
                        inCombat = false;
                    }
                }
            }
        }
    }

    public void UpdateWielder()
    {
        if (grabbable.PrimaryGrabber == null)
        {
            wieldingUser = null;
            Debug.Log("Weapon dropped. wieldingUser == null");
        }
        else
        {
            wieldingUser = grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
            Debug.Log($"Weapon grabbed. wieldingUser: {wieldingUser.userName}");
        }
    }
}