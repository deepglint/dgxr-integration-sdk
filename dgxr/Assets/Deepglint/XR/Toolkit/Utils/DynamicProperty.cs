using System;
using System.Collections.Generic;

namespace Deepglint.XR.Toolkit.Utils
{
    /// <summary>
    /// 属性绑定，属性动态更新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicProperty<T>
    {
        private T _value;

        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && _value == null) return;
                if (value != null && Comparer(value, _value)) return;

                SetValue(value);
                _onValueChanged?.Invoke(value);
            }
        }

        protected virtual T GetValue()
        {
            return _value;
        }

        protected virtual void SetValue(T newValue)
        {
            _value = newValue;
        }

        private Action<T> _onValueChanged = _ => { };

        private Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

        /// <summary>
        /// 注册属性变化，需要UnRegister或者
        /// Register(newCount=>{ }).UnRegisterWhenGameObjectDestroyed(gameobject);
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public IUnRegister Register(Action<T> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            return new DynamicPropertyUnRegister<T>(this, onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(_value);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            _onValueChanged -= onValueChanged;
        }
    }

    public interface IUnRegister
    {
        void UnRegister();
    }

    public class DynamicPropertyUnRegister<T> : IUnRegister
    {
        public DynamicPropertyUnRegister(DynamicProperty<T> bindableProperty, Action<T> onValueChanged)
        {
            DynamicProperty = bindableProperty;
            OnValueChanged = onValueChanged;
        }

        public DynamicProperty<T> DynamicProperty { get; set; }

        public Action<T> OnValueChanged { get; set; }

        public void UnRegister()
        {
            DynamicProperty.UnRegister(OnValueChanged);
            DynamicProperty = null;
            OnValueChanged = null;
        }
    }

    public static class UnRegisterExtension
    {
        public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister,
            UnityEngine.GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger) trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();

            trigger.AddUnRegister(unRegister);

            return unRegister;
        }

        public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
            where T : UnityEngine.Component
        {
            return self.UnRegisterWhenGameObjectDestroyed(component.gameObject);
        }
    }

    public class UnRegisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> _unRegisters = new();

        public void AddUnRegister(IUnRegister unRegister)
        {
            _unRegisters.Add(unRegister);
        }

        public void RemoveUnRegister(IUnRegister unRegister)
        {
            _unRegisters.Remove(unRegister);
        }

        private void OnDestroy()
        {
            foreach (var unRegister in _unRegisters) unRegister.UnRegister();

            _unRegisters.Clear();
        }
    }
}