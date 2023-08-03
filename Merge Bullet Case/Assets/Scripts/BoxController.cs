using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Gameplay;

public class BoxController : MonoBehaviour
{
    public int hp;
    public TMPro.TextMeshPro hpText;
    
    private Vector3 startScale;
    public Rigidbody golds;

    void Start()
    {
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
            transform.GetComponent<Collider>().enabled = false;
            transform.GetComponent<MeshRenderer>().enabled = false;
            golds.isKinematic = false;
            hpText.gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bullet bullet))
        {
            bullet.DeactivateBullet();
            SetHp(bullet.hitValue);
            hitAnim();
            CheckDeath();
        }
    }
}
