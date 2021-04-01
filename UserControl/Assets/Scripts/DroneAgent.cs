using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.Collections;

namespace MBaske
{
    public class DroneAgent : Agent
    {
        [SerializeField]
        private Multicopter multicopter;

        private Bounds bounds;
        private Resetter resetter;

        //The direction an agent will walk during training.
        [Header("Target To Walk Towards")] public Transform dynamicTargetPrefab; //Target prefab to use in Dynamic envs
        private Transform m_Target; //Target the agent will walk towards during training.
        public GameObject frame;
        
        void SpawnTarget(Transform prefab, Vector3 pos)
        {
            m_Target = Instantiate(prefab, pos, Quaternion.identity, transform);
        }
        public override void Initialize()
        {
            multicopter.Initialize();

            bounds = new Bounds(transform.position, Vector3.one * 100);
            resetter = new Resetter(transform);

            SpawnTarget(dynamicTargetPrefab, transform.position); //spawn target
        }

        public override void OnEpisodeBegin()
        {
            resetter.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(multicopter.Inclination);
            sensor.AddObservation(Normalization.Sigmoid(
                multicopter.LocalizeVector(multicopter.Rigidbody.velocity), 0.25f));
            sensor.AddObservation(Normalization.Sigmoid(
                multicopter.LocalizeVector(multicopter.Rigidbody.angularVelocity)));
            
            foreach (var rotor in multicopter.Rotors)
            {
                sensor.AddObservation(rotor.CurrentThrust);
            }

            //Add pos of target relative to orientation cube
            sensor.AddObservation(frame.transform.InverseTransformPoint(m_Target.transform.position));

        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            multicopter.UpdateThrust(actionBuffers.ContinuousActions.Array);

            if (bounds.Contains(multicopter.Frame.position))
            {
                AddReward(multicopter.Frame.up.y);
                // AddReward(multicopter.Rigidbody.velocity.magnitude * -0.2f);
                AddReward(multicopter.Rigidbody.angularVelocity.magnitude * -0.2f);
            }
            else
            {
                EndEpisode();
                // resetter.Reset();
            }
        }

        /// <summary>
        /// Agent touched the target
        /// </summary>
        public void TouchedTarget()
        {
            AddReward(1f);
        }
    }
}

