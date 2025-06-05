using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    private GateManager gateManager;

    private void Start()
    {
        // Find the GateManager in the scene
        gateManager = FindAnyObjectByType<GateManager>();
        if (gateManager == null)
        {
            Debug.LogError($"No GateManager found in the scene! Gate: {gameObject.name}");
        }
        else
        {
            Debug.Log($"GateTrigger initialized for gate: {gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drone"))
        {
            Debug.Log($"Gate trigger entered by drone: {gameObject.name}");
            if (gateManager != null)
            {
                gateManager.GateTriggered(this);
            }
            else
            {
                Debug.LogError($"GateManager is null for gate: {gameObject.name}");
            }
        }
    }
} 