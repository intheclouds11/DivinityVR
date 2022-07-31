using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace HurricaneVR.Framework.ControllerInput
{
    public class HVRTrackedController : MonoBehaviour
    {
        public HVRHandSide HandSide;

        public Vector3 Velocity;// { get; private set; }
        public float VelocityMagnitude;// { get; private set; }
        public float AngularVelocityMagnitude;// { get; private set; }

        public Quaternion DeltaRotationZ { get; private set; }
        public float DeltaEulerZ { get; private set; }
        private Vector3 _previousUp;
        public float DeltaZDisplay;

        private void FixedUpdate()
        {
            var delta = Vector3.SignedAngle(_previousUp, transform.up, transform.forward);
            DeltaEulerZ = delta;
            DeltaRotationZ = Quaternion.Euler(0, 0, delta);
            _previousUp = transform.up;
            if (Mathf.Abs(DeltaEulerZ) > 2)
            {
                DeltaZDisplay = DeltaEulerZ;
            }
        }

        private void LateUpdate()
        {
            Velocity = HVRInputManager.Instance.GetController(HandSide).Velocity;
            VelocityMagnitude = HVRInputManager.Instance.GetController(HandSide).VelocityMagnitude;
            AngularVelocityMagnitude = HVRInputManager.Instance.GetController(HandSide).AngularVelocityMagnitude;
        }
    }
}
