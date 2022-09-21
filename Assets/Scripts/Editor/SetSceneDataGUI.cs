using Assets.Scripts.Component;
using Assets.Scripts.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneData))]
public class SetSceneDataGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SceneData sceneDats = (SceneData) target;

        if (GUILayout.Button("Collect"))
            sceneDats.SpawnPositions =
                FindObjectsOfType<SpawnPositionTag>()
                .Select(x => x.transform.position)
                .ToList();

        EditorUtility.SetDirty(target);
    }
}
