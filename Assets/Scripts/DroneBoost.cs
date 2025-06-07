using UnityEngine;
using RealDronePhysics;

public class DroneBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    public float boostDuration = 3f;
    public float boostThrust = 1.0f; // Direct thrust value for boost
    public float boostPitch = 1.0f; // Direct pitch value for boost

    private DroneInputModule inputModule;
    private DronePhysics dronePhysics;
    private float boostEndTime;
    private bool isBoosting = false;

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
    }

    private void Update()
    {
        // Only allow boost in SelfLeveling mode
        if (dronePhysics.flightConfig != DronePhysicsFlightConfiguration.SelfLeveling)
        {
            if (isBoosting)
            {
                EndBoost();
            }
            return;
        }

        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting)
        {
            StartBoost();
        }

        // Handle active boost
        if (isBoosting)
        {
            if (Time.time >= boostEndTime)
            {
                EndBoost();
            }
            else
            {
                // Use external control for both thrust and pitch
                inputModule.SetExternalControl(boostThrust, boostPitch);
            }
        }
    }

    private void StartBoost()
    {
        isBoosting = true;
        boostEndTime = Time.time + boostDuration;
        Debug.Log("Boost started!");
    }

    private void EndBoost()
    {
        isBoosting = false;
        inputModule.ReleaseExternalControl();
        Debug.Log("Boost ended!");
    }
} 