using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Coin,
    Cube,
    FinishLine,
    Grid,
    Character,
    BulletRange
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType obstacleType;
}
