using UnityEngine;
using UnityEngine.Events;

namespace RealDronePhysics
{
    public enum DronePhysicsFlightConfiguration
    {
        AcroMode, SelfLeveling, AltitudeHold
    };

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DroneInputModule))]
    public class DronePhysics : MonoBehaviour
    {
        public DronePhysicsFlightConfiguration flightConfig = 0;

        public bool armed = true;
        private bool lastArmed = true;

        public DronePhysicsConfiguration physicsConfig;
        public LayerMask hoverEffectLayerMask;

        public UnityEvent OnArmed;
        public UnityEvent OnDisarmed;

        private Transform targetQuad;

        private DronePIDController rotationPID;
        private DronePIDController altitudePID;

        private float targetAltitude;

        [HideInInspector]
        public Vector3 appliedTorque;
        [HideInInspector]
        public Vector3 appliedForce;

        private Rigidbody rb;
        private DroneInputModule inputModule;

        void Start()
        {
            // Get References
            rb = GetComponent<Rigidbody>();
            inputModule = GetComponent<DroneInputModule>();

            // Instantiate Target Transform
            targetQuad = new GameObject("TargetQuad").transform;
            Debug.Log("TargetQuad position: " + targetQuad.rotation);
            targetQuad.parent = transform;
            targetQuad.SetPositionAndRotation(transform.position, transform.rotation * Quaternion.Euler(0, 0, 0));
            rotationPID = new DronePIDController();
            altitudePID = new DronePIDController();
            Debug.Log("TargetQuad position: " + targetQuad.rotation);
        }

        private void FixedUpdate()
        {
            if (armed)
            {
                // Process Input
                switch (flightConfig)
                {
                    case DronePhysicsFlightConfiguration.AcroMode:
                        AcroMode();
                        break;

                    case DronePhysicsFlightConfiguration.SelfLeveling:
                        SelfLeveling();
                        break;

                    case DronePhysicsFlightConfiguration.AltitudeHold:
                        AltitudeHold();
                        break;
                }

                // Process Error, PID
                appliedTorque = CalculatePIDTorque();

                // Apply Torque
                if (appliedTorque.magnitude > 0f)
                    rb.AddRelativeTorque(appliedTorque);

                // Apply Force
                if (appliedForce.magnitude > physicsConfig.thrust)
                    appliedForce = appliedForce.normalized * physicsConfig.thrust;

                rb.AddForce(appliedForce.y * transform.up);

                // Update HoverEffect
                UpdateHoverEffect();

                // Update Rigigbody Configuration
                UpdateRigidbody();

                // Check EventTrigger
                if (lastArmed != armed)
                {
                    if (armed)
                    {
                        OnQuadArmed();
                    }
                    else
                    {
                        OnQuadDisarmed();
                    }

                    lastArmed = armed;
                }

                // Check for Errors
                CheckErrors();
            }
        }

        public void OnPhysicsConfigurationChanged(DronePhysicsConfiguration config)
        {
            physicsConfig = config;
        }
        private void UpdateRigidbody()
        {
            rb.mass = physicsConfig.mass;
            rb.linearDamping = physicsConfig.drag;
            rb.angularDamping = physicsConfig.angularDrag;
        }
        private void CheckErrors()
        {
            if (rb.mass < 0.01f)
                Debug.Log("Drone Mass is unrealistically low. Change in DronePhysicsConfiguration.");
        }

        private void AcroMode()
        {
            targetQuad.localRotation *= Quaternion.Euler(inputModule.pitch * Time.deltaTime, inputModule.yaw * Time.deltaTime, inputModule.roll * Time.deltaTime);
            appliedForce = physicsConfig.thrust * inputModule.thrust * Vector3.one;
        }
        private void SelfLeveling()
        {
            // Pitch and roll
            targetQuad.localRotation = Quaternion.Euler(inputModule.rawPitch * inputModule.ratesConfig.maxAngle, targetQuad.localRotation.eulerAngles.y, inputModule.rawRoll * inputModule.ratesConfig.maxAngle);
            
            // Yaw
            targetQuad.localRotation *= Quaternion.Euler(0f, inputModule.yaw * Time.deltaTime, 0f);

            // Only apply vertical force
            appliedForce = physicsConfig.thrust * inputModule.thrust * Vector3.up;
        }
        private void AltitudeHold()
        {
            targetAltitude += inputModule.rawThrust * Time.deltaTime;

            float error = targetAltitude - transform.position.y;
            float pdForce;

            if(Mathf.Abs(error) > 0.5f)
            {
                targetAltitude = transform.position.y + 0.5f;
            }

            pdForce = altitudePID.CalculatePD(error * Vector3.one, physicsConfig.pAltitude, physicsConfig.dAltitude).y;

            if (pdForce < 0)
                pdForce = 0;

            appliedForce = pdForce * Vector3.up;


            targetQuad.localRotation = Quaternion.Euler(inputModule.rawPitch * inputModule.ratesConfig.maxAngle, targetQuad.localRotation.eulerAngles.y, inputModule.rawRoll * inputModule.ratesConfig.maxAngle);
            targetQuad.localRotation *= Quaternion.Euler(0f, inputModule.yaw * Time.deltaTime, 0f);

        }
        
        private void UpdateHoverEffect()
        {
            if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.3f,hoverEffectLayerMask))
            {
                float hoverFactor = (-(hit.distance / 0.3f) + 1) * 0.25f;
                rb.AddForce(appliedForce.y * transform.up * hoverFactor);
            }
        }
        private Vector3 CalculatePIDTorque()
        {
            Vector3 torque;

            // Calculate Magnitude and Axis
            Quaternion delta = targetQuad.localRotation * Quaternion.Inverse(transform.rotation);

            delta.ToAngleAxis(out float errorMagnitude, out Vector3 axis);

            // Clip errorMagnitude 
            if (errorMagnitude > 180)
                errorMagnitude -= 360;

            if (errorMagnitude < -180)
                errorMagnitude += 360;

            // Calculate Error
            Vector3 error = axis * errorMagnitude;

            // Calculate PID
            torque = rotationPID.CalculatePD(error, physicsConfig.p, physicsConfig.d);
            torque = transform.InverseTransformDirection(torque);

            return torque;
        }

        private void OnQuadArmed()
        {
            OnArmed.Invoke();
            ResetInternals();
        }
        private void OnQuadDisarmed()
        {
            OnDisarmed.Invoke();
            ResetInternals();
        }

        public void ResetInternals()
        {
            targetAltitude = transform.position.y;
            targetQuad.transform.rotation = transform.rotation;
        }
    }
}