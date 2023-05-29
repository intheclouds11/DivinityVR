using HurricaneVR.Framework.ControllerInput;
using UnityEngine;

public class ToggleMovementUI : MonoBehaviour
{
    public bool activated;
    public GameObject movementUI;
    public float timeInTriggerRequired = 1;
    public float timeInTriggerRH;
    public bool inTriggerRH;
    private AudioSource _audioSource;


    private void Update()
    {
        _audioSource = GetComponent<AudioSource>();
        InputCheck();
    }

    private void InputCheck()
    {
        if (inTriggerRH)
        {
            if (timeInTriggerRH < 2)
            {
                timeInTriggerRH += Time.deltaTime;
            }

            if (timeInTriggerRH >= timeInTriggerRequired)
            {
                if (HVRInputManager.Instance.RightController.TriggerButtonState.JustActivated)
                {
                    ToggleVisibility();
                }
            }
        }

        else if (!inTriggerRH)
        {
            if (timeInTriggerRH > 0)
            {
                timeInTriggerRH = 0;
            }
        }
    }

    public void ToggleVisibility()
    {
        _audioSource.Play();
        if (!activated)
        {
            movementUI.SetActive(true);
        }
        else
        {
            movementUI.SetActive(false);
        }

        activated = !activated;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Right Hand"))
        {
            inTriggerRH = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Right Hand"))
        {
            inTriggerRH = false;
        }
    }
}