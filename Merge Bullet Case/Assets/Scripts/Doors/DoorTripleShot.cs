using Gameplay;
using UnityEngine;

namespace Doors
{
    public class DoorTripleShot : DoorController
    {
        private void Start()
        {
            SetSpecialProperties();
            SetGeneralProperties();
        }

        private void SetSpecialProperties()
        {
            doorType = DoorType.TripleShot;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Character character)) return;
        
            CloseDoor();

            levelManager.isTripleShot = true;
        }
    }
}
