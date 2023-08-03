using System.Collections;
using System.Collections.Generic;
using Doors;
using Gameplay;
using UnityEngine;

public class DoorTripleShot : DoorController
{
    void Start()
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
        if (other.TryGetComponent(out Character character))
        {
            CloseDoor();
         
            character.isTripleShot = true;
        }
    }
}
