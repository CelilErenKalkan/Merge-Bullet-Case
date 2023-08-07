using Gameplay;
using UnityEngine;

namespace Doors
{
    public class DoorBulletSize : DoorController
    {
        public float addBulletSize, minBulletSize, maxBulletSize;
        public Material positiveTextBGMat, negativeTextBGMat;
        public Material positiveDoorBGMat, negativeDoorBGMat;

        private void Start()
        {
            SetSpecialProperties();
            SetGeneralProperties();
            SetPositiveNegativeDoors(negativeTextBGMat, negativeDoorBGMat, positiveTextBGMat, positiveDoorBGMat);
            addBulletSize = SetInitialValues(0, 150, (int)addBulletSize);
        }

        private void SetSpecialProperties()
        {
            doorType = DoorType.BulletSize;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                bullet.DeactivateBullet();

                //SetBulletSize
                addBulletSize += bullet.hitValue;
                valueText.text = addBulletSize.ToString();
                ValueTextAnim();

                //SetColor
                if (addBulletSize >= 0)
                    SetPositiveColor(positiveTextBGMat, positiveDoorBGMat);

            }

            if (other.TryGetComponent(out Character character))
            {
                CloseDoor();

                //Change BulletSize
                levelManager.bulletSize += (addBulletSize / 100);
          
                //Set  BulletSize Border
                if (levelManager.bulletSize < minBulletSize)
                    levelManager.bulletSize = minBulletSize;
                else if (levelManager.bulletSize > maxBulletSize)
                    levelManager.bulletSize = maxBulletSize;
            }
        }
    }
}
