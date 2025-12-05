#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class FiringL3CurveBaker
{
    [MenuItem("Tools/FiringL3/Bake Distance Curves (all in scene)")]
    public static void BakeAllFiringCurves()
    {
        // 1. Find the single ShotSystemL3
        var shotSystem = Object.FindAnyObjectByType<ShotSystemL3>();
        if (shotSystem == null)
        {
            Debug.LogError("FiringL3CurveBaker: No ShotSystemL3 found in the scene.");
            return;
        }

        float shotSpeed = shotSystem.ShotSpeed;
        if (shotSpeed <= 0f)
        {
            Debug.LogError("FiringL3CurveBaker: ShotSystemL3.ShotSpeed must be > 0.");
            return;
        }

        // 2. Find all FiringL3 components in the scene (including disabled)
        var firings = Object.FindObjectsByType<FiringL3>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (firings == null || firings.Length == 0)
        {
            Debug.LogWarning("FiringL3CurveBaker: No FiringL3 components found in the scene.");
            return;
        }

        int bakedCount = 0;

        foreach (var firing in firings)
        {
            if (firing == null)
                continue;

            BakeSingleFiring(firing, shotSpeed);
            bakedCount++;

            EditorUtility.SetDirty(firing);
            if (firing.gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(firing.gameObject.scene);
        }

        Debug.Log($"FiringL3CurveBaker: Baked DistanceByYAngle for {bakedCount} FiringL3 component(s).");
    }

    static void BakeSingleFiring(FiringL3 firing, float shotSpeed)
    {
        // Safety
        float step = Mathf.Max(0.1f, firing.CheckAngle); // don't allow 0
        float maxDist = 5000f; // arbitrary max distance

        Transform firePoint = firing.FiringPoint != null ? firing.FiringPoint : firing.transform;
        Vector3 origin = firePoint.position;

        var curve = new AnimationCurve();

        // 3. Sweep 0..360 degrees on Y, inclusive of 360 for overlap
        for (float angle = 0f; angle <= 360f + 0.0001f; angle += step)
        {
            // Direction around global Y axis
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            float distance;
            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist))
            {
                distance = hit.distance;
            }
            else
            {
                // If nothing hit, clamp to max distance
                distance = maxDist;
            }

            float timeToHit = distance / shotSpeed;

            // Use angle as "time" on the curve, and timeToHit as value
            curve.AddKey(new Keyframe(angle, timeToHit));
        }

        // Optionally enforce flat tangents if you don't want smoothing artefacts:
        for (int i = 0; i < curve.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
        }

        firing.DistanceByYAngle = curve;
    }
}
#endif
