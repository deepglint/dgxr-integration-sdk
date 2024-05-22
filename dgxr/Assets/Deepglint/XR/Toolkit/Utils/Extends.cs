using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public static class Extends
    {
        /// <summary>
        /// 通过名称查找子对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="childName">名称</param>
        /// <returns></returns>
        public static GameObject FindChildGameObject(this GameObject obj, string childName)
        {
            if (obj != null)
            {
                var children = obj.GetComponentsInChildren<Transform>(true);

                foreach (var child in children)
                    if (child.name == childName)
                        return child.gameObject;

                Debug.LogWarning($"{obj.name}里找不到名为{childName}的子对象");
                return null;
            }

            return null;
        }
    }
}