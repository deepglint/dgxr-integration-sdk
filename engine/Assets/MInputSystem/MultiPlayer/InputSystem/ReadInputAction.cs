using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using BodySource;
using Moat;

namespace MInputSystem.MultiPlayer.InputSystem
{
    // TODO: Consider renaming this class to follow naming conventions
    public class ReadInputAction: MonoBehaviour
    {
        public InputActionAsset InputActionAssetData;
        public Config _actionConfig;
        public string _actionMap;
        
        void Awake()
        {
            if (_actionConfig == null) return;
            
            var actionMaps = InputActionAssetData.actionMaps;
            
            int index = 0;
            foreach (var actionMap in actionMaps)
            {
                MDebug.Log("Action Map Name: " + actionMap.name + "  " + _actionMap);
                if (actionMap.name != _actionMap) continue;
                var inputActions = actionMap.actions;

                string txt = "";
                foreach (var inputAction in inputActions)
                {
                    index++;
                    // 获取Action的所有Binding
                    var bindings = inputAction.bindings;
    
                    // 遍历所有的Binding
                    foreach (var binding in bindings)
                    {
                        if (binding.path.Contains("Gamepad"))
                        {
                            string actionName = ToTitleCase(inputAction.name); 
                            string keyName = ConvertToCamelCase(binding.path);
                            
                            txt += inputAction.name + " - " + binding.path + "\n";
                            
                            // 根据名称设置动作类型
                            Config.ActionType actionType;
                            if (!Enum.TryParse(actionName, out actionType))
                            {
                                Debug.Log(inputAction.name + " -> " + actionName + " actionType: " + actionType); 
                                continue; // 忽略无效的名称
                            }
                            Config.Key key;
                            Config.keyType type;
                            if (!Enum.TryParse(keyName, out key))
                            {
                                Debug.Log(binding.path + " -> " + keyName + " key: " + key); 
                                continue; // 忽略无效的键信息
                            }
                            type = Config.keyType.Button;
                            
                            _actionConfig.actions.Add(new Config.Action()
                            {
                                action = actionType,
                                key = key,
                                type = type 
                            });
                            
                        }
                    }
                }
                MDebug.Log(txt);
            }

            // SetActionConfig();
        }
        
        private string ConvertToCamelCase(string input)
        {
            // 删除不需要的字符
            string result = input.Replace("/dpad", "/DPad").Replace("<Gamepad>/", "");

            // 将字符串分割为单词
            string[] words = result.Split(new char[] { ' ', '_', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);

            // 转换为大驼峰格式
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            // 合并单词并返回结果
            result = string.Join("", words);

            return result;
        }
        
        private string ToTitleCase(string str)
        {
            char[] chars = str.ToCharArray();
            bool newWord = true;

            for (int i = 0; i < chars.Length; i++)
            {
                if (newWord && char.IsLetter(chars[i]))
                {
                    chars[i] = char.ToUpper(chars[i]);
                    newWord = false;
                }
                else if (char.IsWhiteSpace(chars[i]) || chars[i] == '_' || chars[i] == '-')
                {
                    newWord = true;
                }
            }

            return new string(chars);
        }

        private void SetActionConfig()
        {
            MDebug.Log("=============== Update Action Config ==============");
            foreach (Config.Action action in _actionConfig.actions)
            {
                MDebug.Log(action.ToString());
            }
            
            _actionConfig.actions = new List<Config.Action>()
            {
                new Config.Action()
                {
                    action = Config.ActionType.RaiseOnHand,
                    key = Config.Key.LeftTrigger,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.HandsCross,
                    key = Config.Key.RightTrigger,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToForward,
                    key = Config.Key.ButtonNorth,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToBack,
                    key = Config.Key.ButtonSouth,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToLeft,
                    key = Config.Key.ButtonWest,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToRight,
                    key = Config.Key.ButtonEast,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.SmallSquat,
                    key = Config.Key.DPadUp,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.Butterfly,
                    key = Config.Key.DPadDown,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.FastRun,
                    key = Config.Key.DPadLeft,
                    type = Config.keyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.Freestyle,
                    key = Config.Key.DPadRight,
                    type = Config.keyType.Button
                }
            };
        }
    }
}