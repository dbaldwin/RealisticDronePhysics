using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealDronePhysics
{
    public class DroneAnimation : MonoBehaviour
    {
        [Header("Propellers")]
        public Transform FrontLeft;
        public Transform FrontRight;

        public Transform RearLeft;
        public Transform RearRight;

        private float motorFL;
        private float motorFR;
        private float motorRL;
        private float motorRR;

        [SerializeField]
        private DronePhysics dronePhysics;

        private void Start()
        {
            if (!dronePhysics)
                dronePhysics = GetComponent<DronePhysics>();
        }
        private void Update()
        {
            float thrust = dronePhysics.appliedForce.magnitude * 25f;

            motorFL = thrust + Mathf.Clamp((-dronePhysics.appliedTorque.x - dronePhysics.appliedTorque.z - dronePhysics.appliedTorque.y) * 100f, 0, 100f);
            motorFR = thrust + Mathf.Clamp((-dronePhysics.appliedTorque.x + dronePhysics.appliedTorque.z + dronePhysics.appliedTorque.y) * 100f, 0, 100f);
            motorRL = thrust + Mathf.Clamp((dronePhysics.appliedTorque.x - dronePhysics.appliedTorque.z + dronePhysics.appliedTorque.y) * 100f, 0, 100f);
            motorRR = thrust + Mathf.Clamp((dronePhysics.appliedTorque.x + dronePhysics.appliedTorque.z - dronePhysics.appliedTorque.y) * 100f, 0, 100f);

            if(motorFL != 0)
                FrontLeft.localRotation *=  Quaternion.Euler(0, motorFL, 0);

            if (motorFR != 0)
                FrontRight.localRotation *= Quaternion.Euler(0, motorFR, 0);

            if (motorRL != 0)
                RearLeft.localRotation *= Quaternion.Euler(0, motorRL, 0);

            if (motorRR != 0)
                RearRight.localRotation *= Quaternion.Euler(0, motorRR, 0);
        }
    }
}
