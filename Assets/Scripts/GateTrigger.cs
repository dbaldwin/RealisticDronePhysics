using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    private GateManager gateManager;
    private GateEffects gateEffects;
    private TextMesh gateNumberText;

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

        // Get or add GateEffects component
        gateEffects = GetComponent<GateEffects>();
        if (gateEffects == null)
        {
            gateEffects = gameObject.AddComponent<GateEffects>();
        }

        // Setup gate number text
        SetupGateNumber();
    }

    private void SetupGateNumber()
    {
        // Create a new GameObject for the text
        GameObject textObj = new GameObject("GateNumber");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 0, -0.5f); // Position slightly in front of the gate
        
        // Make text face the same direction as the gate
        textObj.transform.rotation = transform.rotation;
        
        // Counteract parent scaling to keep text size consistent
        Vector3 parentScale = transform.lossyScale;
        textObj.transform.localScale = new Vector3(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z);
        
        // Add and configure TextMesh component
        gateNumberText = textObj.AddComponent<TextMesh>();
        gateNumberText.anchor = TextAnchor.MiddleCenter;
        gateNumberText.alignment = TextAlignment.Center;
        gateNumberText.fontSize = 25;
        gateNumberText.characterSize = 0.2f;
        gateNumberText.color = Color.white;
        
        // Get the gate number from the name (assuming format "Gate X")
        string gateName = gameObject.name;
        if (gateName.StartsWith("Gate "))
        {
            string numberStr = gateName.Substring(5); // Remove "Gate " prefix
            if (int.TryParse(numberStr, out int gateNumber))
            {
                gateNumberText.text = gateNumber.ToString();
            }
        }
    }

    private void LateUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drone"))
        {
            Debug.Log($"Gate trigger entered by drone: {gameObject.name}");
            if (gateManager != null)
            {
                gateManager.GateTriggered(this);
                // Trigger visual effects
                if (gateEffects != null)
                {
                    gateEffects.TriggerEffect();
                }
            }
            else
            {
                Debug.LogError($"GateManager is null for gate: {gameObject.name}");
            }
        }
    }
} 