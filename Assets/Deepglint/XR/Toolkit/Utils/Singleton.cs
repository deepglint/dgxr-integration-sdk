namespace Deepglint.XR.Toolkit.Utils
{
    public class Singleton<T>
        where T : class, new()
    {
        private static T _sInstance = null;

        public static T Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new T();
                }

                return _sInstance;
            }
        }

        public static void Dispose()
        {
            _sInstance = null;
        }

        // 添加构造函数
        protected Singleton()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
        }
    }
}