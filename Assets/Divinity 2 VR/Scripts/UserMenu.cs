using System;
using System.Collections;
using System.Collections.Generic;
using intheclouds;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UserMenu : MonoBehaviour
{
    public Transform originalParent;
    public Vector3 originalLocalPosition;
    public Quaternion originalLocalRotation;
    
    private void OnEnable()
    {
        Debug.Log("on enable called");
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        transform.SetParent(transform.parent.parent, true);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetStats()
    {
        var playerStatsArray = FindObjectsOfType<PlayerStatsSO>();
        foreach (var playerStats in playerStatsArray)
        {
            playerStats.currentHealth = playerStats.maxHealth;
            playerStats.currentAP = playerStats.maxAP;
        }
    }
}
