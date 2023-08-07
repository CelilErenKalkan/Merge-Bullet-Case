using Cinemachine;
using DG.Tweening;
using Editors;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        private Vector3 targetDistance;
        public float followSpeed;
        public Transform target;
        public bool isFollow;
        private int gameplayCamX = 3;
        private int gameplayCamY = 12;
        private Vector3 gameplayCamRot = new Vector3(30, -5, 0);

        public void SetTarget(Transform tr)
        {
            target = tr;

            targetDistance = transform.position - target.transform.position;
            isFollow = true;
        }
        private void LateUpdate()
        {
            if (isFollow)
                transform.position = Vector3.Lerp(transform.position, targetDistance + target.transform.position, followSpeed * Time.deltaTime); //CamFollowDelta = 5
        }

        public void SetGameplayCamera()
        {
            transform.DOMoveX(gameplayCamX, 0.5f);
            transform.DOMoveY(gameplayCamY, 0.5f);
            transform.DORotate(gameplayCamRot, 0.5f);
        }

    }
}