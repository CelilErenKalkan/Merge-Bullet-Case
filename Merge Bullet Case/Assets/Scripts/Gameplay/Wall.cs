using DG.Tweening;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Wall : MonoBehaviour
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
            bullet.TakeDamage(damage);
            Pool.Instance.SpawnObject(transform.position, PoolItemType.WallParticle, null, 1.0f);
            transform.DOScale(transform.localScale * 1.3f, 0.015f).SetEase(Ease.Linear).OnStepComplete(() => transform.DOScale(Vector3.zero, 0.15f));
            GetComponent<Collider>().enabled = false;

        }
    }
}
