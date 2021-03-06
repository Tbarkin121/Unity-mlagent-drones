using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.Collections;
using Unity.MLAgentsExamples;
using UnityEngine.InputSystem;

namespace MBaske
{
    public class NavAgent : Agent
    {
        [SerializeField]
        protected FlyAgent flyAgent;
        PlayerControls controls;
        private Vector3 driveVec;
        private float yawRate;
        private float targetSpeed;
        Vector2 leftStick;
        Vector2 rightStick;
        public float hoverHeight = 5.0f;
        void Awake()
        {
            driveVec = new Vector3(1.0f, 0.0f, 0.0f);
            yawRate = 0.0f;
            controls = new PlayerControls();
        
            controls.Gameplay.XYVec.performed += ctx => leftStick = ctx.ReadValue<Vector2>();
            controls.Gameplay.YYawVec.performed += ctx => rightStick = ctx.ReadValue<Vector2>();
            controls.Gameplay.Speed.performed += ctx => targetSpeed = ctx.ReadValue<float>();
            controls.Gameplay.XYVec.canceled += ctx => leftStick = Vector2.zero;
            controls.Gameplay.YYawVec.canceled += ctx => rightStick = Vector2.zero;
            controls.Gameplay.Speed.canceled += ctx => targetSpeed = 0.0f;
            controls.Gameplay.Enable();
        }

        public override void Initialize()
        {
            flyAgent.navAgentAttached = true;
        }

        public override void OnEpisodeBegin()
        {
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            RaycastHit hit;
            Ray downDetect = new Ray(flyAgent.multicopter.Frame.transform.position, Vector3.down);
            if(Physics.Raycast(downDetect, out hit))
            {
                sensor.AddObservation(hoverHeight-hit.distance);
            }
            
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            var continuousActions = actionBuffers.ContinuousActions;
            Vector3 driveVec = new Vector3(continuousActions[0], continuousActions[1], continuousActions[2]);
            Vector3 world2droneTargetDirection = flyAgent.multicopter.Frame.TransformVector(driveVec);

            driveVec.Normalize();
            flyAgent.driveVector = world2droneTargetDirection;
            flyAgent.TargetSpeed = continuousActions[3];
            flyAgent.TargetRate = continuousActions[4];
        }
        void FixedUpdate()
        {
            RaycastHit hit;
            Ray downDetect = new Ray(flyAgent.multicopter.Frame.transform.position, Vector3.down);
            float TargetReward = 0.0f;
            if(Physics.Raycast(downDetect, out hit))
            {
                switch (hit.collider.tag)
                {
                    case "Center": 
                        TargetReward = 1.0f;
                        break;
                    case "Ring1": 
                        TargetReward = 0.5f;
                        break;
                    case "Ring2":
                        TargetReward = 0.25f;
                        break;
                    case "Ring3":
                        TargetReward = 0.125f;
                        break;
                    default:
                        break;
                }
                SetReward(TargetReward * GetMatchingVelocityReward(hoverHeight, hit.distance, 0.25f)); 
            }
        }

        public override void Heuristic(float[] actionsOut)
        {
            Vector3 targetDirection = new Vector3(leftStick[0], rightStick[1], leftStick[1]);
            Vector3 world2droneTargetDirection = flyAgent.multicopter.Frame.TransformVector(targetDirection);
            // actionsOut[0] = leftStick[0]; //Drive Vector X Component
            // actionsOut[1] = rightStick[1]; //Drive Vector Y Component
            // actionsOut[2] = leftStick[1]; //Drive Vector Z Component
            actionsOut[0] = world2droneTargetDirection[0];
            actionsOut[1] = rightStick[1];
            actionsOut[2] = world2droneTargetDirection[2];
            
            actionsOut[3] = targetSpeed*10; //Target Speed
            print(targetSpeed);

            actionsOut[4] = rightStick[0]*3; //Target Yaw Rate
        }

        public float GetMatchingVelocityReward(float targetVel, float actualVel, float sensitivity)
        {
            float a = 1; //Max Reward
            float b = targetVel;
            float c = sensitivity;
            return a*Mathf.Exp(-(Mathf.Pow((actualVel - b), 2)/(2*c*c))); //Guassian Function centered at target
        }

    }
}
