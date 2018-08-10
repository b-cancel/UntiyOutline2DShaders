﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace object2DOutlines
{
    [CustomEditor(typeof(concaveOut))]
    public class concaveOutEditor : Editor
    {
        //Optimization
        SerializedProperty updateSprite;

        //Debugging
        SerializedProperty showOutline_GOs_InHierarchy;

        //Overlay
        SerializedProperty active_SO;
        SerializedProperty orderInLayer_SO;
        SerializedProperty color_SO;

        //Clipping Mask
        SerializedProperty clipCenter_CM;
        SerializedProperty alphaCutoff_CM;
        SerializedProperty customRange_CM;
        SerializedProperty frontLayer_CM;
        SerializedProperty backLayer_CM;

        //Outline
        SerializedProperty active_O;
        SerializedProperty color_O;
        SerializedProperty orderInLayer_O;
        SerializedProperty scaleWithParentX_O;
        SerializedProperty scaleWithParentY_O;

        SerializedProperty size_O;

        //-----conCAVE
        SerializedProperty edgeCount_O_CAVE_R;
        SerializedProperty rotation_O_CAVE;

        void OnEnable()
        {
            //Optimization
            updateSprite = serializedObject.FindProperty("updateSprite");

            //Debugging
            showOutline_GOs_InHierarchy = serializedObject.FindProperty("showOutline_GOs_InHierarchy");

            //Sprite Outline
            active_SO = serializedObject.FindProperty("active_SO");
            orderInLayer_SO = serializedObject.FindProperty("orderInLayer_SO");
            color_SO = serializedObject.FindProperty("color_SO");

            //Clipping Mask
            clipCenter_CM = serializedObject.FindProperty("clipCenter_CM");
            alphaCutoff_CM = serializedObject.FindProperty("alphaCutoff_CM");
            customRange_CM = serializedObject.FindProperty("customRange_CM");
            frontLayer_CM = serializedObject.FindProperty("frontLayer_CM");
            backLayer_CM = serializedObject.FindProperty("backLayer_CM");

            //Sprite Overlay
            active_O = serializedObject.FindProperty("active_O");
            color_O = serializedObject.FindProperty("color_O");
            orderInLayer_O = serializedObject.FindProperty("orderInLayer_O");
            scaleWithParentX_O = serializedObject.FindProperty("scaleWithParentX_O");
            scaleWithParentY_O = serializedObject.FindProperty("scaleWithParentY_O");

            size_O = serializedObject.FindProperty("size_O");

            //-----conCAVE
            edgeCount_O_CAVE_R = serializedObject.FindProperty("edgeCount_O_CAVE_R");
            rotation_O_CAVE = serializedObject.FindProperty("rotation_O_CAVE");
        }

        public override void OnInspectorGUI()
        {
            //set flags
            EditorStyles.helpBox.wordWrap = true;

            //IDK
            if (GUI.changed)
                EditorUtility.SetDirty(target);

            //link up to our script
            concaveOut script = (concaveOut)target;

            //grab properties from scripts
            serializedObject.Update();

            //---Manual Reset
            if (GUILayout.Button("Reset Variables"))
                script.ManualReset();
            //NOTE: I beleive It either (1) breaks the references of serialized varaibles -OR-  (2) unserializes serialized variables
            EditorGUILayout.HelpBox("DO NOT USE the *RESET* option in the INSPECTOR", MessageType.Warning);
            EditorGUILayout.HelpBox("I require multiple references to GameObjects and Vector2s \nThese references are lost when you use the Unity RESET option", MessageType.None);
            EditorGUILayout.HelpBox("DO NOT USE the *COPY COMPONENT* option in the INSPECTOR", MessageType.Warning);

            EditorGUILayout.Space(); ///-------------------------

            //---Optimization
            EditorGUILayout.PropertyField(updateSprite, new GUIContent("We Update The Sprite"));

            if (script.UpdateSprite == spriteUpdateSetting.Manually)
                if (GUILayout.Button("Update Sprite"))
                    script.updateSpriteData();

            EditorGUILayout.Space(); ///-------------------------

            //---Debugging
            EditorGUILayout.PropertyField(showOutline_GOs_InHierarchy, new GUIContent("Show Outline In Hierarchy"));

            EditorGUILayout.Space(); ///-------------------------

            //---Sprite Overlay
            EditorGUILayout.PropertyField(active_SO, new GUIContent("Activate Sprite Overlay"));

            if (script.Active_SO)
            {
                EditorGUILayout.PropertyField(orderInLayer_SO, new GUIContent("   it's Order In Layer"));
                EditorGUILayout.PropertyField(color_SO, new GUIContent("   it's Color"));
            }

            EditorGUILayout.Space(); ///-------------------------

            //---Clipping Mask
            EditorGUILayout.PropertyField(clipCenter_CM, new GUIContent("Support Semi-Transparency"));

            if (script.ClipCenter_CM)
            {
                EditorGUILayout.PropertyField(alphaCutoff_CM, new GUIContent("   it's Alpha Cut-Off"));
                EditorGUILayout.PropertyField(customRange_CM, new GUIContent("   Use A Custom Range"));
                if (script.CustomRange_CM)
                {
                    EditorGUILayout.PropertyField(frontLayer_CM, new GUIContent("      it's Front Layer"));
                    EditorGUILayout.PropertyField(backLayer_CM, new GUIContent("      it's Back Layer"));
                }
            }

            EditorGUILayout.Space(); ///-------------------------

            //---Sprite Outline
            EditorGUILayout.PropertyField(active_O, new GUIContent("Active Sprite Outline"));

            if (script.Active_O)
            {
                EditorGUILayout.PropertyField(color_O, new GUIContent("   it's Color"));
                EditorGUILayout.PropertyField(orderInLayer_O, new GUIContent("   it's Order In Layer"));
                EditorGUILayout.PropertyField(scaleWithParentX_O, new GUIContent("   Follow Parent X Scale"));
                EditorGUILayout.PropertyField(scaleWithParentY_O, new GUIContent("   Follow Parent Y Scale"));
                EditorGUILayout.PropertyField(size_O, new GUIContent("   it's Size"));
                EditorGUILayout.PropertyField(edgeCount_O_CAVE_R, new GUIContent("   it's # Of Edges"));
                EditorGUILayout.PropertyField(rotation_O_CAVE, new GUIContent("   it's Rotation"));
            }

            //apply modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}