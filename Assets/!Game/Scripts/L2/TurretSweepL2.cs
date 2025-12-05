using UnityEditor;
using UnityEngine;

public class TurretSweepL2 : MonoBehaviour
{
    [Header("Turret")]
    [Tooltip("Child transform to rotate")]
    public Transform turret;

    [Header("Sweep Settings")]
    public float minAngle = -45f;
    public float maxAngle = 45f;

    [Tooltip("How fast the turret sweeps between angles")]
    public float sweepSpeed = 1f; // higher = faster

    [Header("Debug")]
    public float debugLength = 2f;

    Quaternion _baseLocalRotation;

    void Awake()
    {
        if (!turret)
            turret = transform; // fallback

        _baseLocalRotation = turret.localRotation;
    }

    void Update()
    {
        if (!turret) return;

        float t = Mathf.PingPong(Time.time * sweepSpeed, 1f);
        float angle = Mathf.Lerp(minAngle, maxAngle, t);

        turret.localRotation = _baseLocalRotation * Quaternion.Euler(0f, angle, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (!turret) return;

        Vector3 origin = turret.position;
        Vector3 up = turret.up;
        Vector3 forward = turret.forward;

        float width = 5f; // thickness in pixels

        // Min angle line
        Handles.color = Color.green;
        Vector3 dirMin = Quaternion.AngleAxis(minAngle, up) * forward;
        Handles.DrawAAPolyLine(width, origin, origin + dirMin.normalized * debugLength);

        // Max angle line
        Handles.color = Color.red;
        Vector3 dirMax = Quaternion.AngleAxis(maxAngle, up) * forward;
        Handles.DrawAAPolyLine(width, origin, origin + dirMax.normalized * debugLength);
    }

}
