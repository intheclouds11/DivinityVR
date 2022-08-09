using System;
using intheclouds;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserMenu))]
public class UserMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UserMenu userMenu = target as UserMenu;
    }
}