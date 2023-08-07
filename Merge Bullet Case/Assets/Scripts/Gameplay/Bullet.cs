using System.Collections;
using DG.Tweening;
using Doors;
using Editors;
using Managers;
using UnityEngine;
using Grid = Utils.Grid;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        private LevelManager levelManager;
        private GridManager gridManager;
        
        [HideInInspector] public Grid currentGrid, targetGrid;
        public int bulletType, hitValue, gridNum, hp;
        public Vector2 pos;
        public bool isGameBullet, isGunBullet;

        private Bullet targetBullet;
        Vector3 worldPosition;
        private bool isOnTouch, isForwardFire, isRightTripleFire, isLeftTripleFire;
        [HideInInspector] public bool isUnbeatable;
        private const float BulletSpeed = 25;
        private float previousXPos, previousZPos, nextXPos, nextZPos;

        private void Start()
        {
            levelManager = LevelManager.Instance;
            gridManager = GridManager.Instance;
        }

        private void OnEnable()
        {
            if (isGunBullet)
            {
                transform.localScale = Vector3.one;
            }
        }

        private void Update()
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

        private void MoveObject() // Drag control
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

        public void GetGrid()
        {
            if (transform.parent.TryGetComponent(out Grid grid))
            {
                currentGrid = grid;
                currentGrid.gridSit = GridSit.Fill;
                currentGrid.bulletType = bulletType;
            }
        }
        
        public void ResetGrid()
        {
            currentGrid.gridSit = GridSit.Empty;
            gridManager.emptyGrids.Add(currentGrid.gameObject);
            currentGrid.bulletType = 0;
        }

        private void PutDown()
        {
            GetGrid();
            
            if (targetGrid.gridSit.Equals(GridSit.Empty)) //Empty Movement
            {
                //Grid Events
                ResetGrid();
                gridManager.emptyGrids.Remove(targetGrid.gameObject);
                targetGrid.gridSit = GridSit.Fill;
                targetGrid.bulletType = bulletType;

                //Bullet Events
                transform.SetParent(targetGrid.transform);
                transform.localPosition = Vector3.zero;
                pos = targetGrid.pos;
            }
            else
            {
                if (bulletType.Equals(targetGrid.bulletType) &&
                    targetGrid.gameObject != currentGrid.gameObject) //Merge
                {
                    if (targetGrid.transform.GetChild(1).TryGetComponent(out Bullet bullet))
                        targetBullet = bullet;
                    if (bulletType < levelManager.levelEditor.bulletDatas.Length)
                        BulletEditor.Merge(this, targetBullet);
                    else
                        transform.localPosition = Vector3.zero;
                }
                else //Move Initial Place
                    transform.localPosition = Vector3.zero;
            }

            gridNum = transform.parent.GetSiblingIndex();
            levelManager.SaveSystem();
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
                transform.position += (Vector3.forward * BulletSpeed * Time.deltaTime);

            else if (isRightTripleFire)
            {
                transform.position += ((Vector3.forward + Vector3.right * 0.3f) * BulletSpeed * Time.deltaTime);
            }
            else if (isLeftTripleFire)
            {
                transform.position += ((Vector3.forward - Vector3.right * 0.3f) * BulletSpeed * Time.deltaTime);
            }
        }

        private void SetRot()
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
                Actions.LightImpact?.Invoke();
                targetGrid = grid;
            }
            
            if (other.TryGetComponent(out Character character) && isGameBullet)
            {
                Actions.MediumImpact?.Invoke();
                isGameBullet = false;
                character.hitValue += hitValue;
                character.bulletType = bulletType;
                character.isPlay = true;
                CameraManager.Instance.SetGameplayCamera();
                levelManager.StartCharacterMovement(character);
                DeactivateBullet();
                Destroy(gameObject);
            }
            
            if (other.TryGetComponent(out Box box))
            {
                Actions.LightImpact?.Invoke();
                Pool.Instance.SpawnObject(transform.position, PoolItemType.WallParticle, null, 1.0f);
                box.GotHit(hitValue);
                DeactivateBullet();
            }
            
            if (other.TryGetComponent(out Wall wall))
            {
                Actions.LightImpact?.Invoke();
                if (wall.TryGetComponent(out Collider wCollider)) wCollider.enabled = false;
                Pool.Instance.SpawnObject(transform.position, PoolItemType.WallParticle, null, 1.0f);
                wall.transform.DOScale(wall.transform.localScale * 1.3f, 0.015f).SetEase(Ease.Linear).OnStepComplete(() => wall.transform.DOScale(Vector3.zero, 0.15f));
                TakeDamage(wall.damage);
            }
            
            if (other.TryGetComponent(out DoorBulletSize doorBulletSize) 
                     && other.TryGetComponent(out DoorFireRate doorFireRate))
            {
                Actions.LightImpact?.Invoke();
                DeactivateBullet();
            }
        }

        private void TakeDamage(int damage)
        {
            hp -= damage;
            if (hp <= 0)
                DeactivateBullet();
        }
        
        public void DeactivateBullet()
        {
            if (isUnbeatable) return;
            isForwardFire = false;
            isRightTripleFire = false;
            isLeftTripleFire = false;
            StopCoroutine(WaitStopBullet());

            Pool.Instance.DeactivateObject(gameObject, PoolItemType.Bullets);
        }

        private IEnumerator WaitStopBullet()
        {
            yield return new WaitForSeconds(3);
            DeactivateBullet();
        }
    }
}