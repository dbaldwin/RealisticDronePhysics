using UnityEngine;

namespace RealDronePhysics
{
    public class DroneInputModule : MonoBehaviour
    {
        #region
        public DroneRatesConfiguration ratesConfig;

        [Header("Raw Input")]
        [Tooltip("Raw Inputs should be between -1 to 1")]

        [Range(-1f, 1f)]
        public float rawLeftHorizontal = 0f;

        [Range(-1f, 1f)]
        public float rawLeftVertical = 0f;

        [Range(-1f, 1f)]
        public float rawRightHorizontal = 0f;

        [Range(-1f, 1f)]
        public float rawRightVertical = 0f;

        [HideInInspector]
        public float thrust = 0f;
        [HideInInspector]
        public float yaw = 0f;
        [HideInInspector]
        public float pitch = 0f;
        [HideInInspector]
        public float roll = 0f;

        [HideInInspector]
        public float rawThrust = 0f;
        [HideInInspector]
        public float rawYaw = 0f;
        [HideInInspector]
        public float rawPitch = 0f;
        [HideInInspector]
        public float rawRoll = 0f;

        private bool isExternallyControlled = false;
        #endregion

        public void Update()
        {
            CalculateInputWithRatesConfig();
        }

        public void SetExternalControl(float thrustValue, float pitchValue, float rollValue = 0f)
        {
            thrust = thrustValue;
            rawPitch = pitchValue;
            rawRoll = rollValue;
            pitch = TransformInput(pitchValue, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
            roll = TransformInput(rollValue, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
            isExternallyControlled = true;
        }

        public void ReleaseExternalControl()
        {
            isExternallyControlled = false;
        }

        private void CalculateInputWithRatesConfig()
        {
            if (!isExternallyControlled)
            {
                // Implement according to FlightMode
                switch (ratesConfig.mode)
                {
                    case (DroneTransmitterMode.Mode1):
                        rawThrust = rawRightVertical;
                        rawYaw = rawLeftHorizontal;
                        rawPitch = rawLeftVertical;
                        rawRoll = rawRightHorizontal;
                        break;

                    case (DroneTransmitterMode.Mode2):
                        rawThrust = rawLeftVertical;
                        rawYaw = rawLeftHorizontal;
                        rawPitch = rawRightVertical;
                        rawRoll = rawRightHorizontal;
                        break;

                    case (DroneTransmitterMode.Mode3):
                        rawThrust = rawRightVertical;
                        rawYaw = rawRightHorizontal;
                        rawPitch = rawLeftVertical;
                        rawRoll = rawLeftHorizontal;
                        break;

                    case (DroneTransmitterMode.Mode4):
                        rawThrust = rawLeftVertical;
                        rawYaw = rawRightHorizontal;
                        rawPitch = rawRightVertical;
                        rawRoll = rawRightHorizontal;
                        break;
                }

                thrust = (rawThrust + 1) * 0.5f;
                pitch = TransformInput(rawPitch, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
                roll = TransformInput(rawRoll, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
            }

            yaw = TransformInput(rawYaw, ratesConfig.proportionalGain, ratesConfig.exponentialGain);
        }

        public float TransformInput(float input, float p, float e)
        {
            return (input * p) + (Mathf.Pow(input, 2) * Mathf.Sign(input) * e);
        }
    }
}
