using System;
using Stardust.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Stardust.MultiPlayer.InputSystem 
{
    // TODO: Consider renaming this class to follow naming conventions
    public class ReadInputAction: MonoBehaviour
    {
        [FormerlySerializedAs("InputActionAssetData")] public InputActionAsset inputActionAssetData;
        [FormerlySerializedAs("_actionConfig")] public Config actionConfig;
        [FormerlySerializedAs("_actionMap")] public string actionMap;
        
        void Awake()
        {
            if (actionConfig == null) return;
            
            var actionMaps = inputActionAssetData.actionMaps;

            foreach (var actionMapItem in actionMaps)
            {
                MDebug.LogTest("Action Map Name: " + actionMapItem.name + "  " + this.actionMap);
                if (actionMapItem.name != this.actionMap) continue;
                var inputActions = actionMapItem.actions;

                string txt = "";
                foreach (var inputAction in inputActions)
                {
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
                                continue; // 忽略无效的名称
                            }
                            Config.Key key;
                            Config.KeyType type;
                            if (!Enum.TryParse(keyName, out key))
                            {
                                continue; // 忽略无效的键信息
                            }
                            type = Config.KeyType.Button;
                            
                            actionConfig.actions.Add(new Config.Action()
                            {
                                action = actionType,
                                key = key,
                                type = type 
                            });
                            
                        }
                    }
                }
                MDebug.LogTest(txt);
            }
        }
        
        private string ConvertToCamelCase(string input)
        {
            // 删除不需要的字符
            string result = input.Replace("/dpad", "/DPad").Replace("<Gamepad>/", "");

            // 将字符串分割为单词
            string[] words = result.Split(new[] { ' ', '_', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);

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

        #region Test
/*
        private void SetTestActionConfig()
        {
            actionConfig.actions = new List<Config.Action>()
            {
                new Config.Action()
                {
                    action = Config.ActionType.RaiseOnHand,
                    key = Config.Key.LeftTrigger,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.HandsCross,
                    key = Config.Key.RightTrigger,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToForward,
                    key = Config.Key.ButtonNorth,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToBack,
                    key = Config.Key.ButtonSouth,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToLeft,
                    key = Config.Key.ButtonWest,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.ArmToRight,
                    key = Config.Key.ButtonEast,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.SmallSquat,
                    key = Config.Key.DPadUp,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.Butterfly,
                    key = Config.Key.DPadDown,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.FastRun,
                    key = Config.Key.DPadLeft,
                    type = Config.KeyType.Button
                },
                new Config.Action()
                {
                    action = Config.ActionType.Freestyle,
                    key = Config.Key.DPadRight,
                    type = Config.KeyType.Button
                }
            };
        }
*/
        #endregion
        
    }
}