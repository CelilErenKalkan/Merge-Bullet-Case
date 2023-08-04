using UnityEngine;

namespace Utils
{
    public enum ObstacleType
    {
        Coin,
        Cube,
        FinishLine,
    }

    public class Obstacle : MonoBehaviour
    {
        public ObstacleType obstacleType;
    }
}