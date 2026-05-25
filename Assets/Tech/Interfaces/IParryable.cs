using UnityEngine;

public interface IParryable 
{
    void ParryEffect(Vector3 direction, float stunTime, float knockbackDistance);
}
