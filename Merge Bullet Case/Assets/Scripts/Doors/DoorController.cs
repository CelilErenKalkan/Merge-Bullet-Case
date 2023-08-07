using DG.Tweening;
using Managers;
using UnityEngine;

namespace Doors
{
    public enum DoorType
    {
        BulletRange,
        BulletSize,
        FireRate,
        TripleShot
    }
    
    public class DoorController : MonoBehaviour
    {
        protected LevelManager levelManager;
        
        public DoorType doorType;
        public string doorName;
        public bool isPositive;
        private int rnd;
        private bool isFirstColor;
        private Vector3 startSize;
        
        public TMPro.TextMeshPro valueText;
        public TMPro.TextMeshPro nameText;
        public GameObject textBG, doorBG;

        protected void SetGeneralProperties()
        {
            levelManager = LevelManager.Instance;
            nameText.text = doorName;
        }

        protected void SetPositiveNegativeDoors(Material negativeTextBGMat,Material negativeDoorBGMat,Material positiveTextBGMat,Material positiveDoorBGMat)
        {
            rnd = Random.Range(0, 2);
            if (rnd == 0) //Negative
            {
                isPositive = false;
                textBG.GetComponent<Renderer>().sharedMaterial = negativeTextBGMat;
                doorBG.GetComponent<Renderer>().sharedMaterial = negativeDoorBGMat;
            }
            else //Positive
            {
                SetPositiveColor(positiveTextBGMat, positiveDoorBGMat);
            }
        }

        protected void SetPositiveColor (Material positiveTextBGMat, Material positiveDoorBGMat)
        {
            isPositive = true;
            textBG.GetComponent<Renderer>().sharedMaterial = positiveTextBGMat;
            doorBG.GetComponent<Renderer>().sharedMaterial = positiveDoorBGMat;
        }

        protected int SetInitialValues(int min,int max,int  value)
        {
            value = Random.Range(min, max);
            if (!isPositive)
                value = 0 - value;
            valueText.text = value.ToString();
            startSize = valueText.transform.localScale;
            return value;
        }

        protected void ValueTextAnim()
        {
            DOTween.Complete(this);
            valueText.transform.DOScale(startSize * 1.5f, 0.05f).OnStepComplete(() => valueText.transform.DOScale(startSize, 0.05f));
        }

        protected void CloseDoor()
        {
            gameObject.SetActive(false);
        }
    }
}
