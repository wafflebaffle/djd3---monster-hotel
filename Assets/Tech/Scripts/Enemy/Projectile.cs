using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxLifetime = 5f;
    private Rigidbody _rb;
    private Combat _owner;

    private void Start()
    {
        Destroy(gameObject, maxLifetime);

        _rb = GetComponent<Rigidbody>();
        _owner = GetComponentInParent<Combat>();

        transform.parent = null;
        _rb.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_owner != null && collision.gameObject.transform.IsChildOf(_owner.transform))
            return;

        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable target))
        {
            target.TakeDamage(_owner.AttackDamage, _owner);
        }
        Destroy(gameObject);
    }
}
