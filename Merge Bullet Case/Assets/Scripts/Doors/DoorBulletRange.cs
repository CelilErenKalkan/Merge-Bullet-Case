using Gameplay;
using Player;
using UnityEngine;

namespace Doors
{
    public class DoorBulletRange : DoorController
    {
        public int addRange, minRange;
        public Material positiveTextBGMat, negativeTextBGMat;
        public Material positiveDoorBGMat, negativeDoorBGMat;
        private BulletRangeSettings bulletRangeSettings;
        
        private void Start()
        {
            bulletRangeSettings = BulletRangeSettings.Instance;
            SetSpecialProperties();
            SetGeneralProperties();
            SetPositiveNegativeDoors(negativeTextBGMat, negativeDoorBGMat, positiveTextBGMat, positiveDoorBGMat);
            addRange = SetInitialValues(0, 150, addRange);
        }

        private void SetSpecialProperties()
        {
            doorType = DoorType.BulletRange;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                bullet.DeactivateBullet();

                //SetRange
                addRange += bullet.hitValue;
                valueText.text = addRange.ToString();
                ValueTextAnim();

                //SetColor
                if (addRange >= 0)
                    SetPositiveColor(positiveTextBGMat, positiveDoorBGMat);

            }

            if (other.TryGetComponent(out Character character))
            {
                CloseDoor();

                //Change BulletRange
                bulletRangeSettings.transform.localPosition += Vector3.forward * (addRange / 10);

                //Set Min Range
                if (bulletRangeSettings.transform.localPosition.z < minRange)
                    bulletRangeSettings.transform.localPosition = new Vector3(0, 0, minRange);
            }
        }
    }
}
