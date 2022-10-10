using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class Door_Pivot : MonoBehaviour
    {
        [Header("Door should start closed")]
        public Rigidbody body;
        Vector3 closedPosition;
        Quaternion closedRotation;

        [Header("Spring Options")]
        [Tooltip("The door will be locked within this level of open")]
        public float lockThreshold = 0.02f;
        [Tooltip("The door will be closed within this level of open")]
        public float midThreshold = 0.15f;
        [Tooltip("The door will be opened to max direction beyond this level of open")]
        public float maxThreshold = 0.85f;
        [Tooltip("The door will be opened to min direction beyond this level of open")]
        public float minThreshold = 0.85f;
        [Space]

        [Header("Sound Options")]
        public AudioClip doorClose;
        public AudioClip doorOpen;
        public float doorVolume = 1f;

        public UnityEvent OnMax;
        public UnityEvent OnMin;
        public UnityEvent OnMid;
        public UnityEvent OnSpring;
        public UnityEvent OnClose;

        bool max = false;
        bool min = false;
        bool mid = false;
        bool spring = false;
        bool close = true;

        float value = 0;

        private void Awake()
        {
            if (!body && GetComponent<Rigidbody>())
                body = GetComponent<Rigidbody>();
            closedPosition = transform.localPosition;
            closedRotation = transform.localRotation;
        }

//calculate level of open
        public float GetValue() {
          if (GetComponent<HingeJoint>().angle <= 0)
          {
            value = -(GetComponent<HingeJoint>().angle/GetComponent<HingeJoint>().limits.min);
          }
          else
          {
            value = GetComponent<HingeJoint>().angle/GetComponent<HingeJoint>().limits.max;
          }
          return value;
        }

        protected void FixedUpdate()
        {
            if (!min && GetValue() <= -minThreshold)
            {
                Min();
            }

            if (!mid && GetValue() > -minThreshold && GetValue() <= -midThreshold)
            {
                Mid();
            }

            if (!spring && GetValue() > -midThreshold && GetValue() <= -lockThreshold)
            {
                Spring();
            }

            if (!close && GetValue() > -lockThreshold && GetValue() <= lockThreshold)
            {
                Close();
            }

            if (!spring && GetValue() > lockThreshold && GetValue() <= midThreshold)
            {
                Spring();
            }

            if (!mid && GetValue() > midThreshold && GetValue() <= maxThreshold)
            {
                Mid();
            }

            if (!max && GetValue() > maxThreshold)
            {
                Max();
            }

          }

//door will spring to opened position within this range of open
        void Max()
        {
            max = true;
            min = false;
            mid = false;
            spring = false;
            close = false;
            OnMax?.Invoke();
        }

//door will spring to opened position within this range of open
        void Min()
        {
          max = false;
          min = true;
          mid = false;
          spring = false;
          close = false;
          OnMin?.Invoke();
        }

//door is free within this range of open
        void Mid()
        {
          max = false;
          min = false;
          mid = true;
          spring = false;
          close = false;
          OnMid?.Invoke();
        }

//door will spring to closed position within this range of open
        void Spring()
        {
          max = false;
          min = false;
          mid = false;
          spring = true;
          close = false;
          OnSpring?.Invoke();
        }

//door will be locked within this range of open
        void Close()
        {
          max = false;
          min = false;
          mid = false;
          spring = false;
          close = true;
          OnClose?.Invoke();
        }


//change Hinge Joint_Spring to attract door to opened position
        public void SpringToMax()
        {
          HingeJoint hinge = GetComponent<HingeJoint>();
          JointSpring mySpring = hinge.spring;
          mySpring.spring = hinge.spring.spring;
          mySpring.damper = hinge.spring.damper;
          mySpring.targetPosition = hinge.limits.max;
          hinge.spring = mySpring;
          hinge.useSpring = true;
        }

//change Hinge Joint_Spring to attract door to opened position
        public void SpringToMin()
        {
          HingeJoint hinge = GetComponent<HingeJoint>();
          JointSpring mySpring = hinge.spring;
          mySpring.spring = hinge.spring.spring;
          mySpring.damper = hinge.spring.damper;
          mySpring.targetPosition = hinge.limits.min;
          hinge.spring = mySpring;
          hinge.useSpring = true;
        }

//set the door to free position
          public void NoSpring()
          {
            GetComponent<HingeJoint>().useSpring = false;
          }

//change Hinge Joint_Spring to attract door to closed position
        public void SpringToClose()
        {
          HingeJoint hinge = GetComponent<HingeJoint>();
          JointSpring mySpring = hinge.spring;
          mySpring.spring = hinge.spring.spring;
          mySpring.damper = hinge.spring.damper;
          mySpring.targetPosition = 0;
          hinge.spring = mySpring;
          hinge.useSpring = true;
        }

//lock the door
        public void CloseDoor()
        {
          GetComponent<HingeJoint>().useSpring = false;
          transform.localPosition = closedPosition;
          transform.localRotation = closedRotation;
          body.isKinematic = true;
        }

        public void AudioClose()
        {
          if(doorClose)
              AudioSource.PlayClipAtPoint(doorClose, transform.position, doorVolume);
        }

        public void AudioOpen()
        {
          if(doorOpen)
              AudioSource.PlayClipAtPoint(doorOpen, transform.position, doorVolume);
        }

        private void OnDrawGizmosSelected()
        {
            if (!body && GetComponent<Rigidbody>())
                body = GetComponent<Rigidbody>();
        }
    }
}
