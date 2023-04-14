using TMPro;
using UnityEngine;

public class ITCPopup : MonoBehaviour
{
    public bool staticPopup;
    public bool movingPopup;
    public bool growingPopup;
    public float growingSpeed = 0.001f;
    public float translateSpeed;
    public float secondsToDestroy = 2;
    public TextMeshProUGUI TextMeshProUGUI;

    private void Awake()
    {
        Destroy(gameObject, secondsToDestroy);
    }

    private void Update()
    {
        if (!staticPopup)
        {
            if (movingPopup)
            {
                transform.Translate(0, translateSpeed * Time.deltaTime, 0);
            }

            if (growingPopup)
            {
                transform.localScale += (transform.localScale + new Vector3(growingSpeed, growingSpeed, growingSpeed)) * Time.deltaTime;
            }
        }
    }
}