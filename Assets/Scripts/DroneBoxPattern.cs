using UnityEngine;
using RealDronePhysics;

public class DroneBoxPattern : MonoBehaviour
{
    [Header("Pattern Settings")]
    public float stepDuration = 1f; // Duration of each movement step
    public float hoverThrust = 0.5f; // Thrust for hovering
    public float forwardThrust = 0.7f; // Thrust for forward movement
    public float pitchValue = 1.0f; // Maximum pitch value
    public float rollValue = 1.0f; // Maximum roll value

    private DroneInputModule inputModule;
    private DronePhysics dronePhysics;
    private float stepStartTime;
    private int currentStep = 0;
    private bool isPatternActive = false;

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
        // Only allow pattern in SelfLeveling mode
        if (dronePhysics.flightConfig != DronePhysicsFlightConfiguration.SelfLeveling)
        {
            if (isPatternActive)
            {
                EndPattern();
            }
            return;
        }

        // Start pattern on spacebar press
        if (Input.GetKeyDown(KeyCode.Space) && !isPatternActive)
        {
            StartPattern();
        }

        // Handle active pattern
        if (isPatternActive)
        {
            float timeInStep = Time.time - stepStartTime;
            
            if (timeInStep >= stepDuration)
            {
                // Move to next step
                currentStep = (currentStep + 1) % 4;
                stepStartTime = Time.time;
                Debug.Log($"Moving to step {currentStep + 1}");
            }

            // Apply appropriate control based on current step
            switch (currentStep)
            {
                case 0: // Take off and hover
                    inputModule.SetExternalControl(hoverThrust, 0f, 0f);
                    break;
                case 1: // Pitch forward
                    inputModule.SetExternalControl(forwardThrust, pitchValue, 0f);
                    break;
                case 2: // Roll right
                    inputModule.SetExternalControl(forwardThrust, 0f, rollValue);
                    break;
                case 3: // Pitch back
                    inputModule.SetExternalControl(forwardThrust, -pitchValue, 0f);
                    break;
                case 4: // Roll left
                    inputModule.SetExternalControl(forwardThrust, 0f, -rollValue);
                    break;
            }
        }
    }

    private void StartPattern()
    {
        isPatternActive = true;
        currentStep = 0;
        stepStartTime = Time.time;
        Debug.Log("Starting box pattern!");
    }

    private void EndPattern()
    {
        isPatternActive = false;
        inputModule.ReleaseExternalControl();
        Debug.Log("Ending box pattern!");
    }
} 