using System.Collections;
using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using SolClovser.VRDebugGizmos;
using UnityEngine;

namespace intheclouds
{
    public class ITCGrabbable : HVRGrabbable
    {
        public HighlightEffect highlightEffect { get; protected set; }
        public HighlightProfile originalHighlightProfile;
        public bool wasHighlighted;


        protected override void Awake()
        {
            base.Awake();
            highlightEffect = GetComponent<HighlightEffect>();
            hasImpactHandler = GetComponentInParent<ImpactHandler>();
        }

        protected override void OnHoverEnter(HVRGrabberBase grabber)
        {
            base.OnHoverEnter(grabber);

            if (!hoveringGrabber)
            {
                hoveringGrabber = grabber;
                if (grabber is ITCForceGrabber or HVRHandGrabber)
                {
                    OnHandHoverEnter(grabber, this);
                }
                else
                {
                    var socket = grabber as ITCSocket;
                    if (socket && socket.IsValid(this))
                    {
                        OnSocketHoverEnter();
                    }
                }
            }
            else if (grabber is ITCForceGrabber or HVRHandGrabber)
            {
                secondHoveringHand = grabber;
            }
        }

        protected override void OnHoverExit(HVRGrabberBase grabber)
        {
            base.OnHoverExit(grabber);

            if (hoveringGrabber == grabber)
            {
                if (grabber is ITCForceGrabber or HVRHandGrabber)
                {
                    if (secondHoveringHand)
                    {
                        hoveringGrabber = secondHoveringHand;
                        secondHoveringHand = null;
                        return;
                    }

                    OnHandHoverExit(grabber, this);
                }
                else
                {
                    var socket = grabber as ITCSocket;
                    if (socket && socket.IsValid(this))
                    {
                        OnSocketHoverExit();
                    }
                }

                hoveringGrabber = null;
            }
            else if (secondHoveringHand)
            {
                secondHoveringHand = null;
            }
        }

        private void OnHandHoverEnter(HVRGrabberBase grabber, ITCGrabbable grabbable)
        {
            if (!IsHandGrabbed)
            {
                if (highlightEffect)
                {
                    wasHighlighted = highlightEffect.highlighted;
                    highlightEffect.highlighted = true;
                }

                var forceGrabber = grabber as ITCForceGrabber;
                if (forceGrabber)
                {
                    forceGrabber.HoverInfo.HandHovered(grabbable);
                }
                else
                {
                    var handGrabber = grabber as ITCHandGrabber;
                    if (handGrabber)
                    {
                        handGrabber.HoverInfo.HandHovered(grabbable);
                    }
                }
            }
        }

        private void OnHandHoverExit(HVRGrabberBase grabber, ITCGrabbable grabbable)
        {
            if (highlightEffect)
            {
                highlightEffect.highlighted = wasHighlighted;
            }

            var forceGrabber = grabber as ITCForceGrabber;
            if (forceGrabber)
            {
                forceGrabber.HoverInfo.HandUnhovered(grabbable);
            }
            else
            {
                var handGrabber = grabber as ITCHandGrabber;
                if (handGrabber)
                {
                    handGrabber.HoverInfo.HandUnhovered(grabbable);
                }
            }
        }

        private void OnSocketHoverEnter()
        {
            SocketHighlight();
        }

        private void OnSocketHoverExit()
        {
            UnSocketHighlight();
        }

        public void SocketHighlight()
        {
            if (highlightEffect)
            {
                originalHighlightProfile = highlightEffect.profile;
                wasHighlighted = highlightEffect.highlighted;
                highlightEffect.ProfileLoad(HighlightProfileManager.instance.SocketHoverProfile);
                highlightEffect.highlighted = true;
            }
        }

        public void UnSocketHighlight()
        {
            if (highlightEffect)
            {
                highlightEffect.highlighted = false;
                highlightEffect.ProfileLoad(originalHighlightProfile);
            }
        }
        
        // public void FollowHandDirectionAfterThrown(Transform handRaycastTransform)
        // {
        //     if (!followingHandDirection)
        //     {
        //         followingHandDirection = true;
        //         waitForFollowHandDirection = true;
        //         StartCoroutine(CoFollowHandDirectionAfterThrown(handRaycastTransform));
        //     }
        // }
        
        // public IEnumerator CoFollowHandDirectionAfterThrown(Transform handRaycastTransform)
        // {
        //     yield return new WaitForSeconds(0.1f);
        //     
        //     waitForFollowHandDirection = false;
        //
        //     while (followingHandDirection)
        //     {
        //         // todo: close but don't want to add forwards force
        //         // Add force towards direction between hand and grabbable
        //         var handDirInWorldSpace = handRaycastTransform.forward;
        //         var handDirInGrabbableSpace = transform.InverseTransformDirection(handDirInWorldSpace);
        //         var directionFromGrabbableToHand = Vector3.Normalize(handRaycastTransform.position - transform.position);
        //         var dirProjectedOntoHandRightAxis = Vector3.Project(directionFromGrabbableToHand, handRaycastTransform.right);
        //         var dirProjectedOntoHandUpAxis = Vector3.Project(directionFromGrabbableToHand, handRaycastTransform.up);
        //         var newVector = dirProjectedOntoHandRightAxis + dirProjectedOntoHandUpAxis;
        //         
        //         Rigidbody.AddForce(handDirInWorldSpace * 0.2f, ForceMode.VelocityChange);
        //         
        //         VRDebugGizmos.DrawLine(this, "rb", transform.position, transform.position + handDirInWorldSpace, 0.1f, Color.blue);
        //         Debug.Log($"Added force {handRaycastTransform.TransformDirection(handRaycastTransform.forward)} to {name}");
        //         yield return new WaitForFixedUpdate();
        //     }
        // }
    }
}