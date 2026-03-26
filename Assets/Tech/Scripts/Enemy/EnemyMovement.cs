using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float Speed { get; private set; }

    public void SetSpeed(float value)
    {
        Speed = value;
    }
}
