using UnityEngine;

public class ObjectPicker : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupDistance = 0.5f; // Distance to check for objects below
    [SerializeField] private float pickupHeight = 0.2f; // Height offset for picked up object
    [SerializeField] private LayerMask pickupableLayer; // Layer for objects that can be picked up

    private bool isPickupMode = false;
    private GameObject heldObject = null;
    private Vector3 originalObjectPosition;
    private Transform originalParent;
    private Rigidbody heldObjectRigidbody;

    private void Update()
    {
        // Toggle pickup mode with P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (heldObject != null)
            {
                DropObject();
            }
            else
            {
                isPickupMode = !isPickupMode;
            }
        }

        // If in pickup mode and not holding an object, check for objects below
        if (isPickupMode && heldObject == null)
        {
            CheckForPickupableObjects();
        }

        // Update held object position if we're holding something
        if (heldObject != null)
        {
            UpdateHeldObjectPosition();
        }
    }

    private void CheckForPickupableObjects()
    {
        // Cast a ray downward from the drone
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, pickupDistance, pickupableLayer))
        {
            // If we hit something on the pickupable layer, pick it up
            PickupObject(hit.collider.gameObject);
        }
    }

    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        originalObjectPosition = obj.transform.position;
        originalParent = obj.transform.parent;
        heldObjectRigidbody = obj.GetComponent<Rigidbody>();
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            if (heldObjectRigidbody != null)
            {
                // Reset velocity to prevent shooting through floor
                heldObjectRigidbody.linearVelocity = Vector3.zero;
                heldObjectRigidbody.angularVelocity = Vector3.zero;
            }

            heldObject = null;
            heldObjectRigidbody = null;
            isPickupMode = false;
        }
    }

    private void UpdateHeldObjectPosition()
    {
        if (heldObject != null)
        {
            // Position the object below the drone
            heldObject.transform.position = transform.position + Vector3.down * pickupHeight;
            heldObject.transform.rotation = Quaternion.identity;
        }
    }
} 