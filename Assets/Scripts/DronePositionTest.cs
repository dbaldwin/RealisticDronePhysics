using UnityEngine;
using RealDronePhysics;

public class DronePositionTest : MonoBehaviour
{
    private DronePositionController positionController;
    private DronePhysics dronePhysics;

    private void Start()
    {
        positionController = GetComponent<DronePositionController>();
        dronePhysics = GetComponent<DronePhysics>();

        if (positionController == null)
        {
            Debug.LogError("DronePositionController not found on the same GameObject!");
        }
        if (dronePhysics == null)
        {
            Debug.LogError("DronePhysics not found on the same GameObject!");
        }
    }

    private void Update()
    {
        // Only allow position control in SelfLeveling mode
        if (dronePhysics.flightConfig != DronePhysicsFlightConfiguration.SelfLeveling)
        {
            return;
        }

        // Test position control with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            positionController.MoveToPosition(new Vector3(3f, 4f, 5f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            positionController.MoveToPosition(new Vector3(-3f, 2f, -5f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            positionController.MoveToPosition(new Vector3(0f, 1f, 0f));
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            positionController.StopMovement();
        }
    }
} 