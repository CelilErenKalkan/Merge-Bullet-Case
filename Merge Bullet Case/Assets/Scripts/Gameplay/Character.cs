using System.Collections;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Character : MonoBehaviour
    {
        private LevelManager levelManager;
        private GameManager gameManager;
        private float xDifference;
        [SerializeField] private float forwardSpeed = 10f;
        [SerializeField] private float sideSpeed = 18f;
        
        
        private Bullet bulletRef;
        public Transform firePoint;
        [HideInInspector] public int hitValue;
        [HideInInspector] public int bulletType;

        public bool isTripleShot;
        public bool isPlay;
        private bool isTouching = false;
        private bool isFirstTouch;
        private Vector3 currentTouch, firstTouch;

        private void Start()
        {
            levelManager = LevelManager.Instance;
            gameManager = GameManager.Instance;
            StartCoroutine(WaitForFire());
        }
        
        private void Update()
        {
            if (!gameManager.isPlayable || !isPlay) return;
            
            if (Input.GetMouseButtonDown(0))
                isFirstTouch = true;

            if (isFirstTouch)
                MovementController();
        }
        
        private void CallBullet()
        { 
            var bulletObject = Pool.Instance.SpawnObject(firePoint.position, PoolItemType.Bullets, null, bulletType - 1);
            if (bulletObject.TryGetComponent(out Bullet bullet)) bulletRef = bullet;
        }
        
        private IEnumerator WaitForFire()
        {
            while (gameObject.activeInHierarchy)
            {
                if (isTouching && isPlay)
                {
                    CallBullet();
                    bulletRef.ForwardMovement(hitValue);

                    if (isTripleShot)
                    {
                        //Right Bullet
                        CallBullet();
                        bulletRef.TripleRightMovement(hitValue);

                        //LeftBullet Bullet
                        CallBullet();
                        bulletRef.TripleLeftMovement(hitValue);
                    }

                    yield return new WaitForSeconds(levelManager.fireRate * 0.5f);
                }
                
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void MovementController()
        {
            //Forward Movement
            if (isTouching)
                transform.Translate(transform.forward * forwardSpeed * Time.deltaTime);  //15

            //Side Movement
            if (isTouching)
            {
                currentTouch = Input.mousePosition;
                xDifference = (currentTouch.x - firstTouch.x) * 100f / Screen.width;
                xDifference = Mathf.Clamp(xDifference, -1, 1); //Clamp Side acceleration
                Vector3 newPos = transform.position + new Vector3(xDifference, 0, 0);
                transform.position = Vector3.Lerp(transform.position, new Vector3(newPos.x, transform.position.y, transform.position.z), sideSpeed * Time.deltaTime);// MoveSpeed = 11
                //Border Control
                if (transform.position.x <= -4)
                    transform.position = new Vector3(-4, transform.position.y, transform.position.z);
                if (transform.position.x > 4)
                    transform.position = new Vector3(4, transform.position.y, transform.position.z);
            }
            if (Input.GetMouseButton(0))
            {
                isTouching = true;
                firstTouch = Input.mousePosition;
            }


            if (Input.GetMouseButtonUp(0))
            {
                isTouching = false;
            }
        }

        private void EndGameEvents()
        {
            isTouching = isPlay = false;
            levelManager.dataBase.highScore = transform.position.z;
            Actions.LevelEnd?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Coin coin))
            {
                // Get money and update the collectedMoney variable.
                // Optionally, trigger audio/visual effects for collecting coins.
                Pool.Instance.SpawnObject(transform.position, PoolItemType.MoneyParticle, null, 1.0f);
                levelManager.collectedMoney += levelManager.goldValue;
            }
            else if (other.TryGetComponent(out Box box))
            {
                EndGameEvents();
            }
        }
    }
}