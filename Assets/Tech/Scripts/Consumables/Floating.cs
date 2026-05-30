using UnityEngine;

public class Floating : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField]private Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);

    [Header("Floating")]
    [SerializeField]private float floatHeight = 0.25f;
    [SerializeField]private float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotação contínua
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Movimento de flutuação
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}
