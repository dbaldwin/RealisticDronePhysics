using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RealDronePhysics
{
    public class DroneModeSwitcher : MonoBehaviour
    {
        [SerializeField] private DronePhysics dronePhysics;
        [SerializeField] private Button modeButton;

        private void Start()
        {
            if (dronePhysics == null)
                dronePhysics = GetComponent<DronePhysics>();

            if (modeButton != null)
            {
                modeButton.onClick.AddListener(ToggleMode);
                UpdateButtonText();
            }
        }

        public void ToggleMode()
        {
            // Store current position and rotation
            Vector3 currentPosition = dronePhysics.transform.position;
            Quaternion currentRotation = dronePhysics.transform.rotation;

            // Toggle between SelfLeveling and AltitudeHold
            if (dronePhysics.flightConfig == DronePhysicsFlightConfiguration.SelfLeveling)
            {
                dronePhysics.flightConfig = DronePhysicsFlightConfiguration.AltitudeHold;
                // Initialize altitude hold target to current height
                var targetQuad = dronePhysics.transform.Find("Crazyflie");
            }
            else
            {
                dronePhysics.flightConfig = DronePhysicsFlightConfiguration.SelfLeveling;
            }

            Debug.Log("Drone mode switched to: " + dronePhysics.flightConfig);

            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            if (modeButton != null)
            {
                TextMeshProUGUI buttonText = modeButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = dronePhysics.flightConfig == DronePhysicsFlightConfiguration.SelfLeveling 
                        ? "Altitude" 
                        : "Stabilized";
                }
                else
                {
                    Debug.LogError("No TextMeshProUGUI component found on button!");
                }
            }
        }
    }
} 