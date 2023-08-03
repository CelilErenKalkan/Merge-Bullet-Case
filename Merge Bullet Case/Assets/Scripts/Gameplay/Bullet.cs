using System.Collections;
using Editors;
using Managers;
using UnityEngine;
using Utils;
using Grid = Utils.Grid;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        private GameManager gameManager;
        private LevelManager levelManager;
        private GridCreator gridCreator;
        
        [HideInInspector] public Grid currentGridController, targetGridController;
        public int bulletType, hitValue, gridNum, hp;
        public Vector2 pos;
        public bool isGameBullet, isGunBullet;

        private Bullet targetBulletController;
        Vector3 worldPosition;
        private bool isOnTouch, isForwardFire, isRightTripleFire, isLeftTripleFire;
        public float bulletSpeed;
        private float previousXPos, previousZPos, nextXPos, nextZPos;

        private void Start()
        {
            gameManager = GameManager.Instance;
            levelManager = LevelManager.Instance;
            gridCreator = GridCreator.Instance;
        }

        private void OnEnable()
        {
            if (isGunBullet)
            {
                transform.localScale = Vector3.one;
            }
        }

        void Update()
        {
            MoveObject();
            Fire();
            if (isLeftTripleFire || isRightTripleFire || isForwardFire)
                SetRot();
        }

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
            currentGridController = transform.parent.GetComponent<Grid>();
        }

        private void PutDown()
        {
            currentGridController = transform.parent.GetComponent<Grid>();
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
                    targetBulletController = targetGridController.transform.GetChild(1).GetComponent<Bullet>();
                    if (bulletType < levelManager.levelEditor.bulletDatas.Length)
                        BulletEditor.Merge(this, targetBulletController);
                    else
                        transform.localPosition = Vector3.zero;
                }
                else //Move Initial Place
                    transform.localPosition = Vector3.zero;
            }

            gridNum = transform.parent.GetSiblingIndex();
            levelManager.SaveSystem();
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
        }

        public void SetRot()
        {
            previousXPos = transform.position.x;
            previousZPos = transform.position.z;

            transform.forward = Vector3.Lerp(transform.forward,
                new Vector3(previousXPos - nextXPos, 0, previousZPos - nextZPos), 15);

            nextXPos = transform.position.x;
            nextZPos = transform.position.z;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Grid grid))
            {
                targetGridController = grid;
            }
            else if (other.TryGetComponent(out Character character))
            {
                character.hitValue += hitValue;
                character.isPlay = true;
                levelManager.StartCharacterMovement();
                DeactivateBullet();
            }
            else if (!other.TryGetComponent(out Bullet bullet))
            {
                DeactivateBullet();
            }
        }

        public void DeactivateBullet()
        {
            isForwardFire = false;
            isRightTripleFire = false;
            isLeftTripleFire = false;
            StopCoroutine(WaitStopBullet());

            Pool.Instance.DeactivateObject(gameObject, PoolItemType.Bullets);
        }

        IEnumerator WaitStopBullet()
        {
            yield return new WaitForSeconds(3);
            DeactivateBullet();
        }
    }
}