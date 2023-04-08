using System.Collections;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace HurricaneVR.TechDemo.Scripts
{
    public class DemoSlidingDoor : MonoBehaviour
    {
        public Rigidbody LeftDoor;
        public Rigidbody RightDoor;

        public Transform Lock;
        public float LockRotateTime = 1;
        public float LockRotateAngles = 360;

        public float OpenOffset = .5f;
        public float Speed = 1f;

        public AudioClip OpenedClip;

        private bool _opened;
        private bool _moving;


        public void Start()
        {
            //this.ExecuteAfterSeconds(OpenDoors, 1);
        }

        public void Open()
        {
            if (_opened || _moving)
                return;
            _moving = true;
            if(SFXPlayer.Instance) SFXPlayer.Instance.PlaySFX(OpenedClip, transform.position);
            if (LeftDoor && RightDoor)
            {
                StartCoroutine(OpenDoorsRoutine());
            }
            else if (LeftDoor)
            {
                StartCoroutine(OpenDoorRoutine());
            }
        }
        
        public void Close()
        {
            if (!_opened || _moving)
                return;
            _moving = true;
            if(SFXPlayer.Instance) SFXPlayer.Instance.PlaySFX(OpenedClip, transform.position);
            if (LeftDoor && RightDoor)
            {
                StartCoroutine(CloseDoorsRoutine());
            }
            else if (LeftDoor)
            {
                StartCoroutine(CloseDoorRoutine());
            }
        }

        private IEnumerator OpenDoorsRoutine()
        {
            var elapsed = 0f;
            var LockRotateSpeed = LockRotateAngles / LockRotateTime * Time.deltaTime;


            while (elapsed < LockRotateTime)
            {
                Lock.localRotation *= Quaternion.Euler(0f, 0f, LockRotateSpeed);
                yield return null;
                elapsed += Time.deltaTime;
            }


            elapsed = 0f;
            var time = OpenOffset / Speed + (2 * Time.fixedDeltaTime);
            while (elapsed < time)
            {
                yield return new WaitForFixedUpdate();

                var next = Vector3.MoveTowards(LeftDoor.transform.position, LeftDoor.transform.parent.TransformPoint(new Vector3(-OpenOffset, 0f, 0f)), Speed * Time.fixedDeltaTime);
                LeftDoor.MovePosition(next);
                next = Vector3.MoveTowards(RightDoor.transform.position, RightDoor.transform.parent.TransformPoint(new Vector3(OpenOffset, 0f, 0f)), Speed * Time.fixedDeltaTime);
                RightDoor.MovePosition(next);
                elapsed += Time.fixedDeltaTime;
            }
            
            _opened = true;
            _moving = false;
        }
        
        private IEnumerator OpenDoorRoutine()
        {
            var elapsed = 0f;
            var time = OpenOffset / Speed + (2 * Time.fixedDeltaTime);
            while (elapsed < time)
            {
                yield return new WaitForFixedUpdate();

                var next = Vector3.MoveTowards(LeftDoor.transform.position, LeftDoor.transform.parent.TransformPoint(new Vector3(-OpenOffset, 0f, 0f)), Speed * Time.fixedDeltaTime);
                LeftDoor.MovePosition(next);
                elapsed += Time.fixedDeltaTime;
            }
            
            _opened = true;
            _moving = false;
        }
        
        private IEnumerator CloseDoorsRoutine()
        {
            var elapsed = 0f;

            var time = OpenOffset / Speed + (2 * Time.fixedDeltaTime);
            while (elapsed < time)
            {
                yield return new WaitForFixedUpdate();

                var next = Vector3.MoveTowards(LeftDoor.transform.position, LeftDoor.transform.parent.TransformPoint(new Vector3(OpenOffset, 0f, 0f)), Speed * Time.fixedDeltaTime);
                LeftDoor.MovePosition(next);
                next = Vector3.MoveTowards(RightDoor.transform.position, RightDoor.transform.parent.TransformPoint(new Vector3(-OpenOffset, 0f, 0f)), Speed * Time.fixedDeltaTime);
                RightDoor.MovePosition(next);
                elapsed += Time.fixedDeltaTime;
            }
            
            var LockRotateSpeed = LockRotateAngles / LockRotateTime * Time.deltaTime;

            elapsed = 0f;
            while (elapsed < LockRotateTime)
            {
                Lock.localRotation *= Quaternion.Euler(0f, 0f, -LockRotateSpeed);
                yield return null;
                elapsed += Time.deltaTime;
            }
            
            _opened = false;
            _moving = false;
        }
        
        private IEnumerator CloseDoorRoutine()
        {
            var elapsed = 0f;
            var time = OpenOffset / Speed + (2 * Time.fixedDeltaTime);
            while (elapsed < time)
            {
                yield return new WaitForFixedUpdate();

                var next = Vector3.MoveTowards(LeftDoor.transform.position, LeftDoor.transform.parent.TransformPoint(new Vector3(OpenOffset, 0f, 0f)), Speed * Time.fixedDeltaTime);
                LeftDoor.MovePosition(next);
                elapsed += Time.fixedDeltaTime;
            }
            
            _opened = false;
            _moving = false;
        }
    }
}
