using UnityEngine;
using UnityEditor;

namespace RealDronePhysics
{
    public class PCDroneEmulator : MonoBehaviour
    {
        [Header("This Component injects into the InputModule to emulate Controller Inputs with Arrows\n\n" +
                              " - 'Arrows' for Orientation \n - 'Space' for Thrust.\n\n" +
                              "In Altitude Hold Mode, Altitude is Controlled with Mouse Wheel!\n")]

        [SerializeField]
        private DronePhysics dronePhysics;
        [SerializeField]
        private DroneInputModule inputModule;

        private Vector3 startPos = Vector3.zero;
        private Quaternion startRot;

        void Start()
        {
            dronePhysics = GetComponent<DronePhysics>();
            inputModule = GetComponent<DroneInputModule>();

            startPos = transform.position;
            startRot = transform.rotation;
        }

        void Update()
        {
            // Example Population of InputModule
            switch (dronePhysics.flightConfig)
            {
                case (DronePhysicsFlightConfiguration.AcroMode):
                    inputModule.rawRightHorizontal = -Input.GetAxis("Horizontal");
                    inputModule.rawRightVertical = Input.GetAxis("Vertical");
                    inputModule.rawLeftHorizontal = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
                    inputModule.rawLeftVertical = Input.GetAxis("Jump");
                    break;

                case (DronePhysicsFlightConfiguration.SelfLeveling):
                    inputModule.rawRightHorizontal = (Input.GetKey(KeyCode.RightArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                    inputModule.rawRightVertical = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);
                    inputModule.rawLeftHorizontal = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
                    inputModule.rawLeftVertical = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
                    break;

                case (DronePhysicsFlightConfiguration.AltitudeHold):
                    inputModule.rawRightHorizontal = (Input.GetKey(KeyCode.RightArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                    inputModule.rawRightVertical = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);
                    inputModule.rawLeftHorizontal = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
                    inputModule.rawLeftVertical = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
                    break;
            }

            // Respawn on Fire 1
            if (Input.GetButton("Fire1"))
            {
                //Reset Position & Rotation on Respawn
                transform.position = startPos;
                transform.rotation = startRot;

                // Reset Rigidbody on Respawn
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                // Reset Target Rotation on Respawn
                GetComponent<DronePhysics>().ResetInternals();
            }
        }
    }
}
