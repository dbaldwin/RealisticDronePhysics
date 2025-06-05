using UnityEngine;
using TMPro;
using System;
using RealDronePhysics;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;

    [Header("Race Settings")]
    public bool isRaceStarted = false;
    public bool isRaceFinished = false;
    public float startDistanceThreshold = 0.1f; // How far the drone needs to move to start the race

    private float raceTime = 0f;
    private GateManager gateManager;
    private DronePhysics dronePhysics;
    private Vector3 startPosition;

    private void Start()
    {
        gateManager = FindAnyObjectByType<GateManager>();
        if (gateManager == null)
        {
            Debug.LogError("No GateManager found in the scene!");
        }

        dronePhysics = FindAnyObjectByType<DronePhysics>();
        if (dronePhysics == null)
        {
            Debug.LogError("No DronePhysics found in the scene!");
        }
        else
        {
            startPosition = dronePhysics.transform.position;
        }

        // Reset timer display
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (!isRaceStarted && !isRaceFinished && dronePhysics != null)
        {
            float distanceMoved = Vector3.Distance(dronePhysics.transform.position, startPosition);
            Debug.Log($"Distance moved: {distanceMoved:F3}, Threshold: {startDistanceThreshold:F3}");

            if (distanceMoved > startDistanceThreshold)
            {
                StartRace();
            }
        }

        if (isRaceStarted && !isRaceFinished)
        {
            raceTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(raceTime);
            timerText.text = string.Format("{0:00}:{1:00}.{2:000}", 
                timeSpan.Minutes, 
                timeSpan.Seconds, 
                timeSpan.Milliseconds);
        }
    }

    public void StartRace()
    {
        isRaceStarted = true;
        isRaceFinished = false;
        raceTime = 0f;
        UpdateTimerDisplay();
        Debug.Log("Race started!");
    }

    public void FinishRace()
    {
        isRaceFinished = true;
        Debug.Log($"Race finished! Time: {timerText.text}");
    }

    public void ResetRace()
    {
        isRaceStarted = false;
        isRaceFinished = false;
        raceTime = 0f;
        if (dronePhysics != null)
        {
            startPosition = dronePhysics.transform.position;
        }
        UpdateTimerDisplay();
    }
} 