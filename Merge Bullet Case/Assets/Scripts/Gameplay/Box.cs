using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class Box : MonoBehaviour
    {
        public int hp;
        public TMPro.TextMeshPro hpText;
        
        private Vector3 startScale;
        public Rigidbody golds;
        private Collider bCollider;
        private MeshRenderer meshRenderer;
        
        private void Start()
        {
            if (TryGetComponent(out Collider coll)) bCollider = coll;
            if (TryGetComponent(out MeshRenderer mRenderer)) meshRenderer = mRenderer;
            startScale = transform.localScale;
            SetHp(0);
        }

        private void SetHp(int hitValue)
        {
            hp -= hitValue;
            hpText.text = hp.ToString();
            HitAnim();
        }

        private void HitAnim()
        {
            DOTween.Complete(this);
            transform.DOScale(startScale * 1.2f, 0.05f).OnStepComplete(() => transform.DOScale(startScale, 0.05f));
        }

        private void CheckDeath()
        {
            if (hp > 0) return;
            
            bCollider.enabled = false;
            meshRenderer.enabled = false;
            golds.isKinematic = false;
            hpText.gameObject.SetActive(false);
        }

        public void GotHit(int hitValue)
        {
            SetHp(hitValue);
            HitAnim();
            CheckDeath();
        }
    }
}