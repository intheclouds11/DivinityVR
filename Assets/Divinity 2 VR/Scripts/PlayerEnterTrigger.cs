using System;
using HurricaneVR.TechDemo.Scripts;
using UnityEngine;

public class PlayerEnterTrigger : MonoBehaviour
{
    public event Action Trigger;
    public EnemyAI EnemyAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Trigger?.Invoke();
            if (EnemyAI)
            {
                EnemyAI.PlayMoan();
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}