using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class Box : MonoBehaviour
    {
        public int hp;
        public TMPro.TextMeshPro hpText;

        private Bullet currentBullet;
        private Vector3 startScale;
        public Rigidbody golds;
        private Collider _collider;
        private MeshRenderer _meshRenderer;

        private void Start()
        {
            if (TryGetComponent(out Collider collider)) _collider = collider;
            if (TryGetComponent(out MeshRenderer meshRenderer)) _meshRenderer = meshRenderer;
            startScale = transform.localScale;
            SetHp(0);
        }

        private void SetHp(int hitValue)
        {
            hp -= hitValue;
            hpText.text = hp.ToString();
            hitAnim();
        }

        private void hitAnim()
        {
            DOTween.Complete(this);
            transform.DOScale(startScale * 1.2f, 0.05f).OnStepComplete(() => transform.DOScale(startScale, 0.05f));
        }

        private void CheckDeath()
        {
            if (hp <= 0)
            {
                _collider.enabled = false;
                _meshRenderer.enabled = false;
                golds.isKinematic = false;
                hpText.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Bullet bullet)) return;

            currentBullet = bullet;
            currentBullet.DeactivateBullet();
            SetHp(currentBullet.hitValue);
            hitAnim();
            CheckDeath();
        }
    }
}