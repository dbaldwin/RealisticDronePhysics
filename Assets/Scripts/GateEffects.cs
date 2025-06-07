using UnityEngine;

public class GateEffects : MonoBehaviour
{
    [Header("Particle Effects")]
    public ParticleSystem burstEffectPrefab; // This should be a prefab of your particle system
    public float effectDuration = 1f;
    
    private bool isTriggered = false;
    private float triggerTime;
    private ParticleSystem burstEffect;

    private void Start()
    {
        // Create a unique particle system for this gate
        if (burstEffectPrefab != null)
        {
            burstEffect = Instantiate(burstEffectPrefab, transform);
            burstEffect.transform.localPosition = Vector3.zero;
            burstEffect.Stop(); // Make sure it starts stopped
        }
        else
        {
            Debug.LogWarning("No burst effect prefab assigned to GateEffects!");
        }
    }

    private void Update()
    {
        if (isTriggered)
        {
            // Reset triggered state after effect duration
            if (Time.time - triggerTime > effectDuration)
            {
                isTriggered = false;
            }
        }
    }

    public void TriggerEffect()
    {
        isTriggered = true;
        triggerTime = Time.time;

        // Play particle effect
        if (burstEffect != null)
        {
            burstEffect.Stop(); // Stop any existing effect
            burstEffect.Play();
        }
    }

    private void OnDestroy()
    {
        // Clean up the particle system when the gate is destroyed
        if (burstEffect != null)
        {
            Destroy(burstEffect.gameObject);
        }
    }
}