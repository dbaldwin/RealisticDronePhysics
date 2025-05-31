using UnityEngine;


namespace RealDronePhysics
{
    public class DroneSound : MonoBehaviour
    {
        [SerializeField]
        private float pitchFactor = 1f;
        [SerializeField]
        private float volumeFactor = 1f;

        [SerializeField]
        private float pitchOffset = 1f;
        [SerializeField]
        private float volumeOffset = 1f;

        [SerializeField]
        private AudioClip motorSound;
        [SerializeField]
        private DronePhysics dronePhysics;
        [SerializeField]
        private AudioSource source;

        private void Start()
        {
            if(!source)
                source = GetComponent<AudioSource>();

            if(!dronePhysics)
                dronePhysics = GetComponent<DronePhysics>();
        }
        void Update()
        {
            if(dronePhysics.armed)
            {
                if(!source.isPlaying)
                    source.PlayOneShot(motorSound);

                source.pitch = pitchOffset + (dronePhysics.appliedForce.magnitude / dronePhysics.physicsConfig.thrust) * pitchFactor;
                source.volume = volumeOffset + (dronePhysics.appliedForce.magnitude / dronePhysics.physicsConfig.thrust) * volumeFactor;
            }
        }
    }
}