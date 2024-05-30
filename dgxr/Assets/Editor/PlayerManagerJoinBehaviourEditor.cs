using System.Collections.Generic;
using Deepglint.XR.Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlayerManager))]
    [InitializeOnLoad]
    public class PlayerManagerJoinBehaviourEditor : UnityEditor.Editor
    {
        private const string JoinActionBehaviour = "joinAction";
        private const string joinUIBehaviour = "joinUI";
        private readonly Dictionary<string, SerializedProperty> _excludedProperties = new Dictionary<string, SerializedProperty>();
        private readonly List<SerializedProperty> _properties = new List<SerializedProperty>();
        
        private void OnEnable()
        {
            SerializedProperty joinAction = serializedObject.FindProperty(JoinActionBehaviour);
            _excludedProperties.Add(JoinActionBehaviour, joinAction);
            SerializedProperty joinUI = serializedObject.FindProperty(joinUIBehaviour);
            _excludedProperties.Add(joinUIBehaviour, joinUI);
        
            // 获取所有SerializedProperty
            SerializedProperty iterator = serializedObject.GetIterator();
            // 跳过script
            iterator.NextVisible(true); 
        
            while (iterator.NextVisible(false))
            {
                if (!_excludedProperties.ContainsKey(iterator.propertyPath))
                {
                    _properties.Add(iterator.Copy());
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            // 获取目标脚本
            PlayerManager playerManager = (PlayerManager)target;
            
            serializedObject.Update();
            
            // 手动绘制所有属性，排除特定属性
            foreach (SerializedProperty property in _properties)
            {
                EditorGUILayout.PropertyField(property, true);
            }
            
            // 绘制动态属性
            switch (playerManager.JoinBehavior)
            {
                case PlayerJoinBehaviour.JoinFromAction:
                    EditorGUILayout.PropertyField(_excludedProperties[JoinActionBehaviour], new GUIContent("Join Action"));
                    break;
                case PlayerJoinBehaviour.JoinFromUI:
                    EditorGUILayout.PropertyField(_excludedProperties[joinUIBehaviour], new GUIContent("Join UI"));
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
