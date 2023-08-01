using UnityEngine;

namespace Gameplay
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private float forwardSpeed = 10f;
        [SerializeField] private float sideSpeed = 18f;

        private bool isTouching = false;
        private Vector3 firstTouch;

        private void Update()
        {
            HandleInput();
        
            if (isTouching)
                MovementController();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isTouching = true;
                firstTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isTouching = false;
            }
        }

        private void MovementController()
        {
            float horizontalInput = (Input.mousePosition.x - firstTouch.x) * 100f / Screen.width;
            horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f); //Clamp Side acceleration

            Vector3 movement = transform.forward * forwardSpeed * Time.fixedDeltaTime;
            movement += new Vector3(horizontalInput * sideSpeed * Time.fixedDeltaTime, 0f, 0f);

            Vector3 newPos = transform.position + movement;
            newPos.x = Mathf.Clamp(newPos.x, -4f, 4f); //Clamping for Side Movement

            transform.position = newPos;
        }

        private void EndGameEvents()
        {
            // Actions.LevelEnd?.Invoke(); // If Actions.LevelEnd is not set up, this line should be removed.
            // Implement actions to perform when the game ends.
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Obstacle obstacle)) return;

            if (obstacle.obstacleType == ObstacleType.Coin)
            {
                // Get money and update the collectedMoney variable.
                // Optionally, trigger audio/visual effects for collecting coins.
            }
            else
            {
                EndGameEvents();
            }
        }
    }
}