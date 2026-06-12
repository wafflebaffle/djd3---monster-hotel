using UnityEngine;

public class Detector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private LayerMask obstructionMask; // layer for your trees / obstacles

    private FadeObstacle _currentObstacle;

    private void LateUpdate()
    {
        if (!targetTransform) return;

        Vector3 origin = transform.position;
        Vector3 target = targetTransform.position;
        Vector3 dir = target - origin;
        float dist = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, obstructionMask))
        {
            FadeObstacle fade = hit.collider.GetComponent<FadeObstacle>();

            if (fade != null && fade != _currentObstacle)
            {
                if (_currentObstacle != null)
                    _currentObstacle.FadeIn();

                fade.FadeOut();
                _currentObstacle = fade;
            }
        }
        else
        {
            if (_currentObstacle != null)
            {
                _currentObstacle.FadeIn();
                _currentObstacle = null;
            }
        }
    }
}
