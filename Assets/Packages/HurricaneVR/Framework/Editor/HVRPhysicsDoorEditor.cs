using System.Collections;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.ScriptableObjects;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEditor;
using UnityEngine;

namespace HurricaneVR.Framework.Components
{

    [CustomEditor(typeof(HVRPhysicsDoor))]
    public class HVRPhysicsDrawerEditor : UnityEditor.Editor
    {
        private SerializedProperty SP_StartRotation;
        private SerializedProperty SP_EndRotation;
        public HVRPhysicsDoor component;
        private bool _setupExpanded;

        protected void OnEnable()
        {
            SP_StartRotation = serializedObject.FindProperty("StartRotation");
            SP_EndRotation = serializedObject.FindProperty("EndRotation");
            component = target as HVRPhysicsDoor;

        }
        public override void OnInspectorGUI()
        {
            // _setupExpanded = EditorGUILayout.Foldout(_setupExpanded, "Setup Helpers");
            if (true)
            {
                EditorGUILayout.HelpBox("1. Save the joint start rotation of the door.\r\n" +
                                        "2. Save the joint end rotation of the door.\r\n", MessageType.Info);



                DrawButtons("Start", SP_StartRotation);
                DrawButtons("End", SP_EndRotation);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
        
        private void DrawButtons(string label, SerializedProperty property)
        {

            GUILayout.BeginHorizontal();

            if (GUILayout.Button($"Save {label}"))
            {
                property.quaternionValue = component.transform.localRotation;
            }


            if (GUILayout.Button($"GoTo {label}"))
            {
                Undo.RecordObject(component.transform, $"Goto {label}");
                component.transform.localRotation = property.quaternionValue;
            }


            GUILayout.EndHorizontal();
        }
    }
}