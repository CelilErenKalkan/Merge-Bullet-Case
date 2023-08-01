using System.Collections;
using Managers;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        private GameManager gameManager;
        private GridCreator gridCreator;
        [HideInInspector] public GridController currentGridController, targetGridController;
        public int bulletType, hitValue, gridNum, hp;
        public Vector2 pos;
        public bool isUnbeatable, isGameBullet, isGunBullet;

        private Bullet targetBulletController;
        Vector3 worldPosition;
        private bool isOnTouch, isForwardFire, isRightTripleFire, isLeftTripleFire;
        public float bulletSpeed;
        Coroutine StopBullet = null;
        private float previousXPos, previousZPos, nextXPos, nextZPos;

        private void Start()
        {
            gameManager = ObjectManager.GameManager;
            gridCreator = ObjectManager.GridCreator;
        }

        private void OnEnable()
        {
            if (isGunBullet)
            {
                transform.localScale = Vector3.one;
                StopBullet = StartCoroutine(WaitStopBullet());
            }
        }

        void Update()
        {
            MoveObject();
            Fire();
            if (isLeftTripleFire || isRightTripleFire || isForwardFire)
                SetRot();
        }

        #region Movement

        private void OnMouseDown()
        {
            if (!isGameBullet)
                isOnTouch = true;
        }

        private void OnMouseUp()
        {
            if (!isGameBullet)
            {
                isOnTouch = false;
                PutDown();
            }
        }

        private void MoveObject() //Drag control system
        {
            if (isOnTouch)
            {
                Plane plane = new Plane(Vector3.forward, 0);
                float distance;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance);
                }

                transform.position = Vector3.MoveTowards(transform.position, worldPosition, 30 * Time.deltaTime);
            }
        }

        public void GetGridController()
        {
            currentGridController = transform.parent.GetComponent<GridController>();
        }

        private void PutDown()
        {
            currentGridController = transform.parent.GetComponent<GridController>();
            if (targetGridController.gridSit.Equals(GridSit.Empty)) //Empty Movement
            {
                //Grid Events
                ResetGrid();
                gridCreator.emptyGrids.Remove(targetGridController.gameObject);
                targetGridController.gridSit = GridSit.Fill;
                targetGridController.bulletType = bulletType;

                //Bullet Events
                transform.SetParent(targetGridController.transform);
                transform.localPosition = Vector3.zero;
                pos = targetGridController.pos;
            }
            else
            {
                if (bulletType.Equals(targetGridController.bulletType) &&
                    targetGridController.gameObject != currentGridController.gameObject) //Merge
                {
                    targetBulletController = targetGridController.transform.GetChild(0).GetComponent<Bullet>();
                    if (bulletType < gameManager.levelEditor.bulletDatas.Length)
                        gameManager.Merge(this, targetBulletController);
                    else
                        transform.localPosition = Vector3.zero;
                }
                else //Move Initial Place
                    transform.localPosition = Vector3.zero;
            }

            gridNum = transform.parent.GetSiblingIndex();
            gameManager.SaveSystem();
        }

        public void ResetGrid()
        {
            currentGridController.gridSit = GridSit.Empty;
            gridCreator.emptyGrids.Add(currentGridController.gameObject);
            currentGridController.bulletType = 0;
        }

        public void ForwardMovement(int hit)
        {
            hitValue = hit;
            isForwardFire = true;
            StartCoroutine(WaitStopBullet());
        }

        public void TripleRightMovement(int hit)
        {
            hitValue = hit;
            isRightTripleFire = true;
            StartCoroutine(WaitStopBullet());
        }

        public void TripleLeftMovement(int hit)
        {
            hitValue = hit;
            isLeftTripleFire = true;
            StartCoroutine(WaitStopBullet());
        }

        private void Fire()
        {
            if (isForwardFire)
                transform.position += (Vector3.forward * bulletSpeed * Time.deltaTime);

            else if (isRightTripleFire)
            {
                transform.position += ((Vector3.forward + Vector3.right * 0.3f) * bulletSpeed * Time.deltaTime);
            }
            else if (isLeftTripleFire)
            {
                transform.position += ((Vector3.forward - Vector3.right * 0.3f) * bulletSpeed * Time.deltaTime);
            }

            StartCoroutine(WaitOpenBullet());

        }

        IEnumerator WaitOpenBullet()
        {
            yield return new WaitForSeconds(0.15f);
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        public void SetRot() //Updatede �al��acak
        {
            previousXPos = transform.position.x;
            previousZPos = transform.position.z;

            transform.forward = Vector3.Lerp(transform.forward,
                new Vector3(previousXPos - nextXPos, 0, previousZPos - nextZPos), 15);

            nextXPos = transform.position.x;
            nextZPos = transform.position.z;
        }

        #endregion

        #region Collision

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Obstacle obstacle)) return;
            
            if (obstacle.obstacleType == ObstacleType.Grid)
            {
                targetGridController = other.GetComponent<GridController>();
            }

            if (obstacle.obstacleType == ObstacleType.Character)
            {
                other.GetComponent<CharacterManager>().hitValue += hitValue;
                levelManager.StartCharacterMovement(this);
                other.GetComponent<CharacterManager>().isPlay = true;
                Destroy(gameObject);
            }

            if (obstacle.obstacleType == ObstacleType.BulletRange)
            {
                ReplaceQue();
            }
        }

        #endregion

        public void DestroyEvent()
        {
            if (!isUnbeatable)
            {
                Destroy(gameObject);
            }
        }

        public void ReplaceQue()
        {
            isForwardFire = false;
            isRightTripleFire = false;
            isLeftTripleFire = false;
            StopCoroutine(StopBullet);

            poolingManager.replacingBullet(transform.gameObject);
        }

        IEnumerator WaitStopBullet()
        {
            yield return new WaitForSeconds(3);
            ReplaceQue();
        }
    }
}