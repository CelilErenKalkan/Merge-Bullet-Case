using Gameplay;
using Managers;
using UnityEngine;

namespace Doors
{
    public class DoorBulletSize : DoorController
    {
        public LevelManager levelManager;

        public float addBulletSize, minBulletSize, maxBulletSize;
        public Material positiveTextBGMat, negativeTextBGMat;
        public Material positiveDoorBGMat, negativeDoorBGMat;

        void Start()
        {
            levelManager = LevelManager.Instance;
            SetSpecialProperties();
            SetGeneralProperties();
            SetPositiveNegativeDoors(negativeTextBGMat, negativeDoorBGMat, positiveTextBGMat, positiveDoorBGMat);
            addBulletSize = SetInitialValues(0, 150, (int)addBulletSize);
        }

        private void OnEnable()
        {
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
                addBulletSize += tempBulletController.hitValue;
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