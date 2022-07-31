using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CompositeInteraction))]
public class CompositeInteractionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CompositeInteraction compositeInteraction = target as CompositeInteraction;

        if (GUILayout.Button("Run Method"))
        {
            if (Application.isPlaying)
            {
                compositeInteraction?.Interact();
            }
        }
    }
}