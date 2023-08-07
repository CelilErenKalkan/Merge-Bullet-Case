using DG.Tweening;
using Editors;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        // Camera follow variables
        private Vector3 targetDistance;
        private Transform target;
        public float followSpeed;
        public bool isFollow;

        // Gameplay camera settings
        private int gameplayCamX = 3;
        private int gameplayCamY = 12;
        private Vector3 gameplayCamRot = new Vector3(30, -5, 0);

        // Set the target for camera follow
        public void SetTarget(Transform tr)
        {
            target = tr;

            // Calculate initial distance to target
            targetDistance = transform.position - target.transform.position;
            isFollow = true;
        }

        private void LateUpdate()
        {
            // Follow the target smoothly
            if (isFollow)
                transform.position = Vector3.Lerp(transform.position, targetDistance + target.transform.position, followSpeed * Time.deltaTime);
        }

        // Set the camera to gameplay view
        public void SetGameplayCamera()
        {
            // Use DOTween to animate camera position and rotation
            transform.DOMoveX(gameplayCamX, 1);
            transform.DOMoveY(gameplayCamY, 1);
            transform.DORotate(gameplayCamRot, 0.5f);
        }
    }
}