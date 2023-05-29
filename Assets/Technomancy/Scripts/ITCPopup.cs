using System.Linq;
using Assets.HurricaneVR.Framework.Shared.Utilities;
using HurricaneVR.Framework.Core;
using intheclouds;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class ITCPopup : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;
    public Canvas MainCanvas;
    public bool scaleWithDistance;
    [ShowIf(nameof(scaleWithDistance))]
    public AnimationCurve scaleFactor;
    public bool movingPopup;
    [ShowIf(nameof(movingPopup))]
    public float translateSpeed;
    public bool growingPopup;
    [ShowIf(nameof(growingPopup))]
    public float growingSpeed = 0.001f;
    public float secondsToDestroy = 2;
    public Transform target;

    private Transform cam;
    private LookAt lookAt;
    private float originalFontSize;
    private Transform originalParent;


    private void Awake()
    {
        cam = HVRManager.Instance.Camera.transform;
        lookAt = GetComponent<LookAt>();
        originalFontSize = TextMeshProUGUI.fontSize;
        originalParent = transform.parent;
        if (secondsToDestroy > 0)
        {
            Destroy(gameObject, secondsToDestroy);
        }
    }
    
    private void Update()
    {
        if (scaleWithDistance)
        {
            TextMeshProUGUI.fontSize = originalFontSize * scaleFactor.Evaluate(Vector3.Distance(transform.position, cam.position));
        }

        if (movingPopup)
        {
            transform.Translate(0, translateSpeed * Time.deltaTime, 0);
        }

        if (growingPopup)
        {
            transform.localScale += (transform.localScale + new Vector3(growingSpeed, growingSpeed, growingSpeed)) * Time.deltaTime;
        }

        if (target)
        {
            PositionHoverInfo();
        }
    }

    public void UnParent(bool unParent)
    {
        if (!originalParent)
        {
            originalParent = transform.parent;
        }

        transform.parent = unParent ? null : originalParent;
    }

    public void HandHovered(ITCGrabbable grabbable)
    {
        if (grabbable.TryGetComponent(out IHoverableItem hoverableItem))
        {
            // First disable canvas and enable it next frame
            lookAt.enabled = true;
            MainCanvas.enabled = false;
            this.ExecuteNextUpdate(() => MainCanvas.enabled = true);
            
            grabbable.Destroyed.AddListener(OnTargetDestroyed);
            target = grabbable.transform;
            TextMeshProUGUI.text = hoverableItem.GetHoverInfo();
            UnParent(true);
            PositionHoverInfo();
        }
    }

    public void HandUnhovered(ITCGrabbable grabbable, bool force = false)
    {
        // If this popup is assigned to the grabbable, hide popup.
        if (force || target == grabbable.transform)
        {
            lookAt.enabled = false;
            MainCanvas.enabled = false;
            
            target = null;
            UnParent(false);
        }
        // Otherwise its the secondary hoverer so hide the primary hoverer popup
        else
        {
            foreach (var itcPopup in LocalUserObjects.Instance.HUDController.HoverInfoList.Where(itcPopup => itcPopup != this))
            {
                itcPopup.HandUnhovered(grabbable, true);
                return;
            }
        }
    }

    private void PositionHoverInfo()
    {
        var dir = target.position - HVRManager.Instance.Camera.position;
        // todo: need to consider size of grabbable. Look into collider and renderer bounds
        transform.position = target.position - dir.normalized * 0.1f + new Vector3(0, 0.2f, 0);
    }
    
    private void OnTargetDestroyed(HVRGrabbable grabbable)
    {
        HandUnhovered(null, true);
    }
}