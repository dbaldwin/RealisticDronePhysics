using UnityEngine;
using UnityEngine.InputSystem;

namespace RealDronePhysics
{
    public class XBOXControllerInput : MonoBehaviour
    {
        [Header("This Component injects into the InputModule and uses Inputs from XBOX Gamepad\n\n\n")]

        [SerializeField]
        private DronePhysics dronePhysics;
        [SerializeField]
        private DroneInputModule inputModule;

        private Vector3 startPos = Vector3.zero;
        private Quaternion startRot;
        private Gamepad gamepad;

        void Start()
        {
            dronePhysics = GetComponent<DronePhysics>();
            inputModule = GetComponent<DroneInputModule>();
            gamepad = Gamepad.current;

            startPos = transform.position;
            startRot = transform.rotation;
        }

        void Update()
        {
            if (gamepad == null)
            {
                gamepad = Gamepad.current;
                if (gamepad == null) return;
            }

            // Inject Inputs from Joystick into InputModule
            inputModule.rawLeftHorizontal = gamepad.leftStick.x.ReadValue();
            inputModule.rawLeftVertical = gamepad.leftStick.y.ReadValue();

            inputModule.rawRightHorizontal = -gamepad.rightStick.x.ReadValue();
            inputModule.rawRightVertical = gamepad.rightStick.y.ReadValue();

            // Respawn on Fire 1 (using the A button on Xbox controller)
            if(gamepad.buttonSouth.wasPressedThisFrame)
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
