using intheclouds;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpiritWander))]
public class SpiritWanderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SpiritWander spiritWander = target as SpiritWander;

        if (GUILayout.Button("ToggleSpiritForm"))
        {
            if (Application.isPlaying)
            {
                if (spiritWander != null) spiritWander.ToggleSpiritForm();
            }
        }
    }
}