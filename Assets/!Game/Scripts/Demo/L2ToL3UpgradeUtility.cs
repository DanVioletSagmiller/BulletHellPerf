#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.SceneManagement;

public static class L2ToL3UpgradeUtility
{
    [MenuItem("Tools/Upgrade/L2 -> L3 (Loaded Scenes)")]
    public static void UpgradeLoadedScenes()
    {
        int sceneCount = SceneManager.sceneCount;
        if (sceneCount == 0)
        {
            Debug.LogWarning("No scenes are loaded.");
            return;
        }

        int totalReplaced = 0;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
                continue;

            bool changed = false;

            foreach (var root in scene.GetRootGameObjects())
            {
                changed |= UpgradeHierarchy(root, ref totalReplaced);
            }

            if (changed)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                Debug.Log($"Upgraded L2 -> L3 in scene: {scene.path}");
            }
        }

        Debug.Log($"L2->L3 upgrade complete. Total components replaced: {totalReplaced}");
    }

    static bool UpgradeHierarchy(GameObject root, ref int totalReplaced)
    {
        bool changed = false;
        var allTransforms = root.GetComponentsInChildren<Transform>(true);

        foreach (var t in allTransforms)
        {
            var go = t.gameObject;
            if (ReplaceComponent<FiringL2, FiringL3>(go, ref totalReplaced))
                changed = true;
            if (ReplaceComponent<TurretSweepL2, TurretSweepL3>(go, ref totalReplaced))
                changed = true;
        }

        return changed;
    }

    static bool ReplaceComponent<TFrom, TTo>(GameObject go, ref int totalReplaced)
        where TFrom : Component
        where TTo : Component
    {
        var oldComponents = go.GetComponents<TFrom>();
        if (oldComponents == null || oldComponents.Length == 0)
            return false;

        foreach (var oldComp in oldComponents)
        {
            Undo.RegisterFullObjectHierarchyUndo(go, $"Replace {typeof(TFrom).Name} with {typeof(TTo).Name}");

            // Copy values from old component
            ComponentUtility.CopyComponent(oldComp);

            // Add new component and paste values into it
            var newComp = go.AddComponent<TTo>();
            ComponentUtility.PasteComponentValues(newComp);

            // Remove old component
            Undo.DestroyObjectImmediate(oldComp);

            totalReplaced++;
        }

        return true;
    }
}
#endif
