using UnityEngine;
using RealDronePhysics;

public class DronePositionController : MonoBehaviour
{
    [Header("Position Control Settings")]
    public float positionP = 0.5f;  // Proportional gain for position control
    public float positionD = 0.1f;  // Derivative gain for position control
    public float maxTiltAngle = 15f; // Maximum tilt angle in degrees
    public float hoverThrust = 0.5f; // Base thrust for hovering
    public float positionThreshold = 0.1f; // How close to target position is considered "reached"

    private DroneInputModule inputModule;
    private DronePhysics dronePhysics;
    private DronePIDController xPID;
    private DronePIDController yPID;
    private DronePIDController zPID;
    private Vector3 targetPosition;
    private bool isMovingToTarget = false;

    private void Start()
    {
        inputModule = GetComponent<DroneInputModule>();
        dronePhysics = GetComponent<DronePhysics>();
        
        if (inputModule == null)
        {
            Debug.LogError("DroneInputModule not found on the same GameObject!");
        }
        if (dronePhysics == null)
        {
            Debug.LogError("DronePhysics not found on the same GameObject!");
        }

        // Initialize PID controllers
        xPID = new DronePIDController();
        yPID = new DronePIDController();
        zPID = new DronePIDController();

        // Set initial target position to current position
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isMovingToTarget || dronePhysics.flightConfig != DronePhysicsFlightConfiguration.SelfLeveling)
        {
            return;
        }

        // Calculate position error
        Vector3 currentPosition = transform.position;
        Vector3 positionError = targetPosition - currentPosition;

        // Check if we've reached the target position
        if (positionError.magnitude < positionThreshold)
        {
            // Hover in place
            inputModule.SetExternalControl(hoverThrust, 0f, 0f);
            isMovingToTarget = false;
            Debug.Log("Reached target position!");
            return;
        }

        // Calculate PID outputs for each axis
        Vector3 xControl = xPID.CalculatePD(positionError, positionP, positionD);
        Vector3 yControl = yPID.CalculatePD(positionError, positionP, positionD);
        Vector3 zControl = zPID.CalculatePD(positionError, positionP, positionD);

        // Calculate pitch and roll based on horizontal position error
        float pitch = Mathf.Clamp(xControl.x, -maxTiltAngle, maxTiltAngle) / maxTiltAngle;
        float roll = Mathf.Clamp(zControl.z, -maxTiltAngle, maxTiltAngle) / maxTiltAngle;

        // Calculate thrust based on vertical position error
        float thrust = hoverThrust + yControl.y;
        thrust = Mathf.Clamp(thrust, 0f, 1f);

        // Apply control inputs
        inputModule.SetExternalControl(thrust, pitch, roll);
    }

    public void MoveToPosition(Vector3 position)
    {
        targetPosition = position;
        isMovingToTarget = true;
        Debug.Log($"Moving to position: {position}");
    }

    public void StopMovement()
    {
        isMovingToTarget = false;
        inputModule.ReleaseExternalControl();
    }
} 