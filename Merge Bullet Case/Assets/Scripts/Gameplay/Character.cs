using System.Collections;
using Managers;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class Character : MonoBehaviour
    {
        // References to managers
        private LevelManager levelManager;
        private GameManager gameManager;

        // Movement speed variables
        private float xDifference;
        [SerializeField] private float forwardSpeed = 10f;
        [SerializeField] private float sideSpeed = 18f;

        // Bullet-related variables
        private Bullet bulletRef;
        public Transform firePoint;
        [HideInInspector] public int hitValue;
        [HideInInspector] public int bulletType;

        // Gameplay control variables
        public bool isPlay;
        private bool isTouching = false;
        private bool isFirstTouch;
        private Vector3 currentTouch, firstTouch;

        private void Start()
        {
            // Initialize references to managers
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
            // Spawn and initiate bullet
            var bulletObject = Pool.Instance.SpawnObject(firePoint.position, PoolItemType.Bullets, null, bulletType - 1);
            if (bulletObject.TryGetComponent(out Bullet bullet)) bulletRef = bullet;
        }

        private IEnumerator WaitForFire()
        {
            while (gameObject.activeInHierarchy)
            {
                if (isTouching && isPlay)
                {
                    // Fire forward
                    CallBullet();
                    bulletRef.ForwardMovement(hitValue);

                    if (levelManager.isTripleShot)
                    {
                        // Fire right
                        CallBullet();
                        bulletRef.TripleRightMovement(hitValue);

                        // Fire left
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
            // Forward Movement
            if (isTouching)
                transform.Translate(transform.forward * forwardSpeed * Time.deltaTime);

            // Side Movement
            if (isTouching)
            {
                currentTouch = Input.mousePosition;
                xDifference = (currentTouch.x - firstTouch.x) * 100f / Screen.width;
                xDifference = Mathf.Clamp(xDifference, -1, 1);
                Vector3 newPos = transform.position + new Vector3(xDifference, 0, 0);
                transform.position = Vector3.Lerp(transform.position, new Vector3(newPos.x, transform.position.y, transform.position.z), sideSpeed * Time.deltaTime);

                // Border Control
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
            // Handle end of the game
            isTouching = isPlay = false;
            levelManager.dataBase.highScore = transform.position.z;
            Actions.LevelEnd?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Money money))
            {
                // Collect money
                Destroy(money.gameObject);
                Actions.LightImpact?.Invoke();
                Pool.Instance.SpawnObject(money.transform.position, PoolItemType.MoneyParticle, null, 1.0f);
                levelManager.collectedMoney += levelManager.goldValue;
            }
            else if (other.TryGetComponent(out Box box))
            {
                // Trigger endgame events
                EndGameEvents();
            }
        }
    }
}