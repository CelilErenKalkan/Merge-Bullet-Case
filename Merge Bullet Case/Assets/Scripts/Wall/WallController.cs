using DG.Tweening;
using Gameplay;
using UnityEngine;

namespace Wall
{
    public class WallController : MonoBehaviour
    {
        public int damage;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                BulletCollisionEvent(bullet);
            }
        }

        private void BulletCollisionEvent(Bullet bullet)
        {
            bullet.hp -= damage;
            if (bullet.hp <= 0)
                bullet.DeactivateBullet();
        
            transform.DOScale(transform.localScale * 1.3f, 0.015f).SetEase(Ease.Linear).OnStepComplete(() => transform.DOScale(Vector3.zero, 0.15f));
            GetComponent<Collider>().enabled = false;

        }
    }
}
