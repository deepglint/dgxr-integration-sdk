using System.Collections.Generic;
using Deepglint.XR.Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlayerManager))]
    public class PlayerManagerJoinBehaviourEditor : UnityEditor.Editor
    {
        private readonly List<SerializedProperty> _excludedProperties = new List<SerializedProperty>();
        private readonly List<SerializedProperty> _properties = new List<SerializedProperty>();

        // private void OnEnable()
        // {
        //     _excludedProperties.Add(serializedObject.FindProperty("joinAction"));
        //     _excludedProperties.Add(serializedObject.FindProperty("joinUI"));
        //
        //     // 获取所有SerializedProperty
        //     SerializedProperty iterator = serializedObject.GetIterator();
        //     iterator.NextVisible(true); 
        //
        //     while (iterator.NextVisible(false))
        //     {
        //         foreach (var properity in _excludedProperties)
        //         {
        //             if (iterator.propertyPath != properity.propertyPath)
        //             {
        //                 _properties.Add(iterator.Copy());
        //             }
        //         }
        //     }
        // }
        
        public override void OnInspectorGUI()
        {
            // 获取目标脚本
            // PlayerManager playerManager = (PlayerManager)target;
            //
            DrawDefaultInspector();
            serializedObject.Update();
            
            //
            // // 手动绘制所有属性，排除特定属性
            // foreach (SerializedProperty property in _properties)
            // {
            //     EditorGUILayout.PropertyField(property, true);
            // }
            //
            // // 绘制动态属性
            // switch (playerManager.JoinBehavior)
            // {
            //     case PlayerJoinBehaviour.JoinFromAction:
            //         EditorGUILayout.PropertyField(serializedObject.FindProperty("joinAction"), new GUIContent("Join Action"));
            //         break;
            //     case PlayerJoinBehaviour.JoinFromUI:
            //         EditorGUILayout.PropertyField(serializedObject.FindProperty("joinUI"), new GUIContent("Join UI"));
            //         break;
            // }
            //
            serializedObject.ApplyModifiedProperties();
        }
    }
}