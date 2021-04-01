using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.Collections;
using Unity.MLAgentsExamples;

namespace MBaske
{
    public class FlyAgent : Agent
    {
        [SerializeField]
        public Multicopter multicopter;

        private Bounds bounds;
        private Resetter resetter;

        [SerializeField]
        public GameObject frame;
        private Rigidbody rb;
        [SerializeField]
        private float radius;
        public float dist2touch = 0.3f;

        public Vector3 driveVector;
        public float VelocitySensitivity = 1;
        public bool navAgentAttached = false;


        //The walking speed to try and achieve
        private float m_TargetSpeed = m_maxSpeed;
        private float m_TargetRate = m_maxRate;
        [Header("Fly Speed")]
        [Range(0.1f, m_maxSpeed)]
        [SerializeField]
        [Tooltip(
            "The speed the agent will try to match.\n\n" +
            "TRAINING:\n" +
            "For VariableSpeed envs, this value will randomize at the start of each training episode.\n" +
            "Otherwise the agent will try to match the speed set here.\n\n" +
            "INFERENCE:\n" +
            "During inference, VariableSpeed agents will modify their behavior based on this value " +
            "whereas the CrawlerDynamic & CrawlerStatic agents will run at the speed specified during training "
        )]
        const float m_maxSpeed = 20; //The max walking speed


        [Header("Yaw Rate")]
        [Range(0.1f, m_maxRate)]
        [SerializeField]
        [Tooltip(
            "The speed the agent will try to match.\n\n" +
            "TRAINING:\n" +
            "For VariableSpeed envs, this value will randomize at the start of each training episode.\n" +
            "Otherwise the agent will try to match the speed set here.\n\n" +
            "INFERENCE:\n" +
            "During inference, VariableSpeed agents will modify their behavior based on this value " +
            "whereas the CrawlerDynamic & CrawlerStatic agents will run at the speed specified during training "
        )]
        const float m_maxRate = 20; //The max walking speed


        //The current target fly speed. 
        public float TargetSpeed
        {
            get { return m_TargetSpeed; }
            set { m_TargetSpeed = Mathf.Clamp(value, 0, m_maxSpeed); }
        }

        //The current target yaw rate. 
        public float TargetRate
        {
            get { return m_TargetRate; }
            set { m_TargetRate = Mathf.Clamp(value, -m_maxRate, m_maxRate); }
        }
        
        //Should the agent sample a new goal velocity each episode?
        //If true, TargetWalkingSpeed will be randomly set between 0.1 and m_maxWalkingSpeed in OnEpisodeBegin()
        //If false, the goal velocity will be m_maxWalkingSpeed
        private bool m_RandomizeWalkSpeedEachEpisode;
        //The direction an agent will walk during training.
        [Header("Target To Walk Towards")] public Transform TargetPrefab; //Target prefab to use in Dynamic envs
        public Transform staticTargetPrefab; //Target prefab to use in Static envs
        private Transform m_Target; //Target the agent will walk towards during training.
        //This will be used as a stabilized model space reference point for observations
        //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
        OrientationCubeController m_OrientationCube;
        //The indicator graphic gameobject that points towards the target
        DirectionIndicator m_DirectionIndicator;

        public LineRenderer velocity_line;
        public LineRenderer direction_line;
        public LineRenderer rate_line;

        StatsRecorder m_Recorder;
        EnvironmentParameters m_ResetParams;
        public override void Initialize()
        {
            m_OrientationCube = GetComponentInChildren<OrientationCubeController>();
            m_DirectionIndicator = GetComponentInChildren<DirectionIndicator>();

            multicopter.Initialize();
            rb = frame.GetComponent<Rigidbody>();
            bounds = new Bounds(transform.position, Vector3.one * 1000);
            resetter = new Resetter(multicopter.transform);
            
            TargetSpeed = 0;
            TargetRate = 0;
            driveVector = new Vector3(1.0f, 0.0f, 0.0f);
            
            SpawnTarget(TargetPrefab, transform.position); //spawn target
            RandomizeTarget();

            m_Recorder = Academy.Instance.StatsRecorder;
            m_ResetParams = Academy.Instance.EnvironmentParameters;

        }

        public override void OnEpisodeBegin()
        {
            resetter.Reset();
            RandomizeTarget();
            UpdateOrientationObjects();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            Vector3 currentVelocityVec = multicopter.Rigidbody.velocity;
            float currentVelocity = currentVelocityVec.magnitude;

            sensor.AddObservation(multicopter.Inclination);
            //Body speed normalized
            sensor.AddObservation(Normalization.Sigmoid(multicopter.LocalizeVector(multicopter.Rigidbody.velocity), 0.25f));
            // sensor.AddObservation(TargetWalkingSpeed);
            //target speed with same normalization
            // sensor.AddObservationNormalization.Sigmoid(TargetSpeed, 0.25f));
            sensor.AddObservation(Normalization.Sigmoid(TargetSpeed - multicopter.Rigidbody.velocity.magnitude, 0.25f)); 

            sensor.AddObservation(Normalization.Sigmoid(multicopter.LocalizeVector(multicopter.Rigidbody.angularVelocity)));
            // sensor.AddObservation(Normalization.Sigmoid(TargetRate));
            sensor.AddObservation(Normalization.Sigmoid(TargetRate - multicopter.LocalizeVector(multicopter.Rigidbody.angularVelocity).y));
            //TargetRate - ActualRate = RateError (Maybe a better observation?)
            foreach (var rotor in multicopter.Rotors)
            {
                sensor.AddObservation(rotor.CurrentThrust);
            }

            sensor.AddObservation(multicopter.Frame.InverseTransformVector(driveVector));
            

        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            multicopter.UpdateThrust(actionBuffers.ContinuousActions.Array);
            UpdateDriveVector();

            if (bounds.Contains(multicopter.Rigidbody.position))
            {
                // AddReward(multicopter.Frame.up.y);
                // AddReward(multicopter.Rigidbody.velocity.magnitude * -0.2f);
                // AddReward(multicopter.Rigidbody.angularVelocity.magnitude * -0.05f); //try not to spin too much
                
                // AddReward(1.0f); //Universal Basic Income so bots stop suiciding...

                Vector3 velVector = multicopter.Rigidbody.velocity;
                float frameSpeed = velVector.magnitude;
                velVector.Normalize();
                // float velVecReward = Mathf.Max(Vector3.Dot(velVector, driveVector), 0);
                float velVecReward = Vector3.Dot(velVector, driveVector);
    
                // float velMatchReward = Mathf.Exp(-VelocitySensitivity * Mathf.Abs(TargetSpeed - frameSpeed));
                float velMatchReward = GetMatchingVelocityReward(TargetSpeed, frameSpeed, 0.5f);

                float currentYawRate = multicopter.LocalizeVector(multicopter.Rigidbody.angularVelocity).y;
                float rotRateMatchReward = GetMatchingVelocityReward(TargetRate, currentYawRate, 0.5f); //I hope this is correct... 
                AddReward(velVecReward * velMatchReward * rotRateMatchReward); // No Negitive Rewards
                // AddReward(Vector3.Dot(velVector, driveVector)); //Ranges from 1 for velocity following the driveVector to -1 for going opposite driveVector

                // AddReward(velMatchReward); //Goes to 1 as it nears target, goes to 0 as it diverges 
                // AddReward(AgentUtil.Sigmoid(1-Mathf.Abs(TargetWalkingSpeed - frameSpeed))); //gives a positive number for being close to the target speed, goes to zero as velocity diverges from target


                Vector3 vec2target = (m_Target.position - rb.position);
                float dist2target = vec2target.magnitude;

                if (dist2target < dist2touch)
                {
                    TouchedTarget();
                    RandomizeTarget();
                }
            }
            else
            {
                // resetter.Reset();
                EndEpisode();
            }
        }

        /// <summary>
        /// Agent touched the target
        /// </summary>
        public void TouchedTarget()
        {
            AddReward(1f);
        }

        private void RandomizeTarget()
        {
            m_Target.position = transform.position + Random.insideUnitSphere * radius;
            // float RandomDefaultSpeed = Random.Range(0.1f, m_maxSpeed);
            // TargetSpeed = m_ResetParams.GetWithDefault("speed", RandomDefaultSpeed);
            if(!navAgentAttached)
            {
                TargetSpeed = Academy.Instance.EnvironmentParameters.GetWithDefault("speed", 1);
                TargetRate  = Academy.Instance.EnvironmentParameters.GetWithDefault("rate", 0);
            }
            
        }

        private void UpdateDriveVector()
        {
            if(!navAgentAttached)
            {
                driveVector = (m_Target.transform.position - frame.transform.position);
            }
            driveVector.Normalize();
        }

        void SpawnTarget(Transform prefab, Vector3 pos)
        {
            m_Target = Instantiate(prefab, pos, Quaternion.identity, transform);
            m_DirectionIndicator.targetToLookAt = m_Target;
        }

        void UpdateOrientationObjects()
        {
            m_OrientationCube.UpdateOrientation(multicopter.Rigidbody.transform, m_Target);
            m_DirectionIndicator.heightOffset = multicopter.Rigidbody.position[1];
            if (m_DirectionIndicator)
            {
                m_DirectionIndicator.MatchOrientation(m_OrientationCube.transform);
            }
        }

        void FixedUpdate()
        {
            UpdateOrientationObjects();
            Vector3 lineOffset = new Vector3(0.0f, 1.0f, 0.0f);
            velocity_line.SetPosition(0, rb.position);
            velocity_line.SetPosition(1, rb.position + rb.velocity);
            direction_line.SetPosition(0, rb.position);
            direction_line.SetPosition(1, rb.position + driveVector);
            rate_line.SetPosition(0, rb.position);
            Vector3 yaw_vec = new Vector3(0.0f, 0.0f, 0.0f);
            rate_line.SetPosition(1, rb.position + yaw_vec);
            
            
            // direction_line.SetPosition(1, rb.position + multicopter.Frame.InverseTransformVector(driveVector));

            if((Time.frameCount % 100) == 0)
            {
                m_Recorder.Add("Velocity Error", TargetSpeed - rb.velocity.magnitude);
                m_Recorder.Add("Rate Error", TargetRate - multicopter.Rigidbody.angularVelocity.y);
            }

        }

        /// <summary>
        /// Normalized value of the difference in actual speed vs goal walking speed.
        /// </summary>
        public float GetMatchingVelocityReward(float targetVel, float actualVel, float sensitivity)
        {
            float a = 1; //Max Reward
            float b = targetVel;
            float c = sensitivity;
            return a*Mathf.Exp(-(Mathf.Pow((actualVel - b), 2)/(2*c*c))); //Guassian Function centered at target
        }
    }
}