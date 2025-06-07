using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        // Sort gates by their number in the name
        var sortedTriggers = gateTriggers.OrderBy(trigger => {
            string name = trigger.gameObject.name;
            if (name.StartsWith("Gate "))
            {
                string numberStr = name.Substring(5); // Remove "Gate " prefix
                if (int.TryParse(numberStr, out int gateNumber))
                {
                    return gateNumber;
                }
            }
            return int.MaxValue; // Put invalid names at the end
        }).ToArray();

        // Add gates in sorted order
        foreach (GateTrigger trigger in sortedTriggers)
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
        if (gates == null) return;

        for (int i = 0; i < gates.Count; i++)
        {
            if (gates[i] == null || gates[i].gateTransform == null) continue;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gates[i].gateTransform.position, 0.5f);

            #if UNITY_EDITOR
            Handles.Label(gates[i].gateTransform.position + Vector3.up, $"Gate {i}");
            #endif
        }
    }
} 