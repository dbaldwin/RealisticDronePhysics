using UnityEngine;

namespace RealDronePhysics
{
    public enum DroneTransmitterMode
    {
        Mode1, Mode2, Mode3, Mode4
    };

    [System.Serializable]
    public class DroneRatesConfiguration
    {
        public DroneRatesConfiguration(DroneRatesConfiguration ratesConfig)
        {
            proportionalGain = ratesConfig.proportionalGain;
            exponentialGain = ratesConfig.exponentialGain;
            mode = ratesConfig.mode;
            maxAngle = ratesConfig.maxAngle;
        }

        [Header("Gains [Deg/s]")]
        public float proportionalGain = 45f;
        public float exponentialGain = 0f;

        [Header("Flight Mode")]
        public DroneTransmitterMode mode = DroneTransmitterMode.Mode2;

        [Header("Self Leveling [Deg]")]
        public float maxAngle = 15f;
    }
}