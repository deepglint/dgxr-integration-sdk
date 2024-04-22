
namespace Stardust.Scripts
{
    public class MSingleton<T>
        where T : class, new()
    {
        private static T _sInstance;
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
        protected MSingleton()
        {
        }

        static MSingleton()
        {
            _sInstance = null;
        }

        protected virtual void Initialize()
        {
        }
    }
}