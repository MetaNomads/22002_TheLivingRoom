using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
  [System.Serializable]
  public struct StepEvent
  {
    public int step;
    public UnityEvent OnStepEnter;
    public UnityEvent OnStepExit;
  }

  public class Lever : MonoBehaviour
  {
    [Header("Spring Options")]
    [Tooltip("The door will be opened beyond this level of open")]
    public float openThreshold = 0.85f;
    [Tooltip("The lever will be reset within this level of open")]
    public float closeThreshold = 0.05f;
    [Space]

    public int stepCount = 0;
    public int startStep = 0;
    private int prevStepCount = -1;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public StepEvent[] stepEvents;

    bool open = false;
    bool close = true;

    float value = 0;
    bool grab = false;

    private int currStep = -1;
    private int prevStep = -1;

    private float minimum;
    private float maximum;
    float[] stepMarkers;

    HingeJoint joint;
    Quaternion startRotation;

    private void Start() {
      joint = GetComponent<HingeJoint>();
      startRotation = transform.localRotation;
      if(startStep <= 0) return;
      FindSteps();
      SetSpring(startStep - 1);
    }

    void Update() {
        AdjustStep();
    }

    protected void FixedUpdate(){
        if (!open && GetValue() <= -openThreshold)
        {
            Open();
        }

        if (!close && GetValue() > -closeThreshold && GetValue() <= closeThreshold)
        {
            Close();
        }

        if (!open && GetValue() > openThreshold)
        {
            Open();
        }
    }

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

    void AdjustStep() {
        if(stepCount <= 0) return;

        FindSteps();
        SetSpring(FindCurrentStep());
    }

    bool FindSteps() {
        if(prevStepCount == stepCount) return false;

        prevStepCount = stepCount;

        stepMarkers = new float[stepCount];

        minimum = joint.limits.min;
        maximum = joint.limits.max;

        float step = GetStep();

        for(int i = 0; i < stepCount; i++) {
            stepMarkers[stepCount - i - 1] = minimum + (i * step);
        }

        return true;
    }

    public void SetSpring(int step)
    {
        joint.transform.localRotation *= Quaternion.Euler(joint.axis  * stepMarkers[step]);

        currStep = step;
        JointSpring jointSpring = joint.spring;
        jointSpring.targetPosition = stepMarkers[step];
        joint.spring = jointSpring;
    }

    public void SetSpring(float stepRotation)
    {
        JointSpring jointSpring = joint.spring;
        jointSpring.targetPosition = stepRotation;
        joint.spring = jointSpring;
    }

    float FindCurrentStep() {
        float checkValue = GetValue() * GetRange();
        for(int i = 0; i < stepCount; i++) {
            if(checkValue >= GetMinimumStep(i) && checkValue <= GetMaximumStep(i)) {
                currStep = i;
                if(currStep != prevStep) {
                    Step();
                    prevStep = currStep;
                }

                return stepMarkers[i];
            }
        }

        return 0;
    }

    float GetStep() => (Mathf.Abs(minimum) + Mathf.Abs(maximum)) / (stepCount - 1);
    float GetRange() => (Mathf.Abs(minimum) + Mathf.Abs(maximum)) / 2;
    float GetMinimumStep(int index) => stepMarkers[index] - (GetStep() / 2);
    float GetMaximumStep(int index) => stepMarkers[index] + (GetStep() / 2);

    //Events
    //lever will close the door
    void Close()
    {
      close = true;
      open = false;
      OnClose?.Invoke();
    }

    //lever will open the door
    void Open()
    {
      close = false;
      open = true;
      OnOpen?.Invoke();
    }

    void Step()
    {
      for(int i = 0; i < stepEvents.Length; i++)
      {
        if(stepEvents[i].step == currStep + 1)
        {
          stepEvents[i].OnStepEnter?.Invoke();
        }
        else if(stepEvents[i].step == prevStep + 1)
        {
          stepEvents[i].OnStepExit?.Invoke();
        }
      }
    }

    void LeverReturn()
    {
      
    }

//Functions
//disable lever when not grabbing
    private void forceGrab()
    {
      if (!grab && GetComponent<HingeJoint>().angle == 0)
      {
        GetComponent<Rigidbody>().isKinematic = true;
      }
    }

    public void resetLever()
    {
      //transform.localPosition = closedPosition;
      GetComponent<Rigidbody>().transform.localRotation = startRotation;
    }
  }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class Lever : MonoBehaviour
    {
        [Header("Lever should start closed")]
        public Rigidbody body;
        Vector3 closedPosition;
        Quaternion closedRotation;

        [Header("Spring Options")]
        [Tooltip("The lever will be closed beyond this level of open")]
        public float openThreshold = 0.85f;
        public float closeThreshold = 0.05f;
        [Space]

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        bool close = true;
        bool open = false;

        float value = 0;
        bool grab = false;

        private void Awake()
        {
            if (!body && GetComponent<Rigidbody>())
                body = GetComponent<Rigidbody>();
            closedPosition = transform.localPosition;
            closedRotation = transform.localRotation;
        }

        protected void FixedUpdate()
        {
          if (!open && GetValue() <= -openThreshold)
          {
              Open();
          }

          if (!close && GetValue() > -closeThreshold && GetValue() <= closeThreshold)
          {
              Close();
          }

          if (!open && GetValue() > openThreshold)
          {
              Open();
          }
          forceGrab();
        }


//Functions
//disable lever when not grabbing
        private void forceGrab()
        {
            if (!grab && GetComponent<HingeJoint>().angle == 0)
            {
              GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        public void resetLever()
        {
          //transform.localPosition = closedPosition;
          GetComponent<Rigidbody>().transform.localRotation = closedRotation;
        }

//calculate level of open
        public float GetValue()
        {
          if (GetComponent<HingeJoint>().angle <= 0)
          {
            value = GetComponent<HingeJoint>().angle/GetComponent<HingeJoint>().limits.min;
          }
          else
          {
            value = GetComponent<HingeJoint>().angle/GetComponent<HingeJoint>().limits.max;
          }
          return value;
        }

        public void onGrab()
        {
          grab = true;
        }

        public void offGrab()
        {
          grab = false;
        }

//Events
//lever will close the door
        void Close()
        {
            close = true;
            open = false;
            OnClose?.Invoke();
        }

//lever will open the door
        void Open()
        {
          close = false;
          open = true;
          OnOpen?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            if (!body && GetComponent<Rigidbody>())
                body = GetComponent<Rigidbody>();
        }
    }
}
*/
