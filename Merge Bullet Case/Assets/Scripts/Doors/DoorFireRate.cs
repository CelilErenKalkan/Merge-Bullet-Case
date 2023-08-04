using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Doors;
using Gameplay;
using Managers;
using UnityEngine;

public class DoorFireRate : DoorController
{
    private LevelManager levelManager;
    
    public float addFireRate,minFireRate,maxFireRate;
    public Material positiveTextBGMat, negativeTextBGMat;
    public Material positiveDoorBGMat, negativeDoorBGMat;

    private void Start()
    {
        levelManager = LevelManager.Instance;
        SetSpecialProperties();
        SetGeneralProperties();
        SetPositiveNegativeDoors(negativeTextBGMat, negativeDoorBGMat, positiveTextBGMat, positiveDoorBGMat);
        addFireRate= SetInitialValues(0,150, (int)addFireRate);
    }

    private void SetSpecialProperties()
    {
        doorType = DoorType.FireRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bullet bullet))
        {
            bullet.DeactivateBullet();
           
            //SetFireRate
            addFireRate += bullet.hitValue;
            valueText.text = addFireRate.ToString();
            ValueTextAnim();

            //SetColor
            if (addFireRate >= 0)
                SetPositiveColor(positiveTextBGMat, positiveDoorBGMat);

        }

        if (other.TryGetComponent(out Character character))
        {
            CloseDoor();

            //Change BulletRange
            levelManager.fireRate -=  (addFireRate / 1000);
           
            //Set Border FireRate
            if (levelManager.fireRate < minFireRate)
                levelManager.fireRate = minFireRate;
            else if(levelManager.fireRate > maxFireRate)
                levelManager.fireRate = maxFireRate;
        }
    }
}
