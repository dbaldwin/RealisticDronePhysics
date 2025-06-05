using UnityEngine;
using System.Collections.Generic;

public class GateManager : MonoBehaviour
{
    [System.Serializable]
    public class Gate
    {
        public string gateName;
        public Transform gateTransform;
        public bool isTriggered;
    }

    [Header("Gate Configuration")]
    public List<Gate> gates = new List<Gate>();
    public int currentGateIndex = 0;
    public bool isCourseComplete = false;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool autoFindGates = true;
    public bool showGateNumbers = true;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No GameManager found in the scene!");
        }

        if (autoFindGates)
        {
            FindAllGates();
        }
        
        // Reset all gates at start
        ResetGates();
        Debug.Log($"GateManager initialized with {gates.Count} gates");
    }

    private void FindAllGates()
    {
        // Find all GateTriggers in the scene
        GateTrigger[] gateTriggers = FindObjectsByType<GateTrigger>(FindObjectsSortMode.None);
        Debug.Log($"Found {gateTriggers.Length} gates in the scene");

        // Clear existing gates
        gates.Clear();

        // Sort gates by their X position (assuming gates are arranged left to right)
        System.Array.Sort(gateTriggers, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // Add gates in sorted order
        foreach (GateTrigger trigger in gateTriggers)
        {
            Gate newGate = new Gate
            {
                gateName = trigger.gameObject.name,
                gateTransform = trigger.transform,
                isTriggered = false
            };
            gates.Add(newGate);
            Debug.Log($"Added gate: {newGate.gateName}");
        }
    }

    public void GateTriggered(GateTrigger trigger)
    {
        Debug.Log($"GateManager received trigger from: {trigger.gameObject.name}");
        
        // Find which gate was triggered by comparing transforms
        for (int i = 0; i < gates.Count; i++)
        {
            if (gates[i].gateTransform == trigger.transform)
            {
                Debug.Log($"Found matching gate at index {i}, current index is {currentGateIndex}");
                
                // Only trigger if this is the next gate in sequence
                if (i == currentGateIndex)
                {
                    Debug.Log($"Correct gate sequence! Triggering gate {i}");
                    TriggerGate();
                }
                else
                {
                    Debug.Log($"Wrong gate! Expected gate {currentGateIndex} but triggered gate {i}");
                }
                return;
            }
        }
        
        Debug.LogWarning($"Trigger received from unknown gate: {trigger.gameObject.name}");
    }

    private void TriggerGate()
    {
        if (currentGateIndex < gates.Count)
        {
            gates[currentGateIndex].isTriggered = true;
            currentGateIndex++;

            if (currentGateIndex >= gates.Count)
            {
                isCourseComplete = true;
                Debug.Log("Course Complete!");
                if (gameManager != null)
                {
                    gameManager.FinishRace();
                }
            }
            else
            {
                Debug.Log($"Gate {gates[currentGateIndex - 1].gateName} triggered! Next gate: {gates[currentGateIndex].gateName}");
            }
        }
    }

    public void ResetGates()
    {
        currentGateIndex = 0;
        isCourseComplete = false;
        foreach (var gate in gates)
        {
            gate.isTriggered = false;
        }
        Debug.Log("Gates have been reset");
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        // Draw lines between gates to show the course
        for (int i = 0; i < gates.Count - 1; i++)
        {
            if (gates[i].gateTransform != null && gates[i + 1].gateTransform != null)
            {
                Gizmos.color = gates[i].isTriggered ? Color.green : Color.yellow;
                Gizmos.DrawLine(gates[i].gateTransform.position, gates[i + 1].gateTransform.position);
            }
        }

        // Draw current gate target and gate numbers
        for (int i = 0; i < gates.Count; i++)
        {
            if (gates[i].gateTransform != null)
            {
                if (i == currentGateIndex)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(gates[i].gateTransform.position, 1f);
                }
                
                if (showGateNumbers)
                {
                    // Draw the gate number
                    UnityEditor.Handles.Label(gates[i].gateTransform.position + Vector3.up, $"Gate {i}");
                }
            }
        }
    }
} 