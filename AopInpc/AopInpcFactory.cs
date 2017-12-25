using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace AopInpc
{
    public static class AopInpcFactory
    {
        public static T Create<T>(params object[] args) where T : class, INotifyPropertyChangedCaller
        {
            var inpcType = typeof(T);
            Debug.Assert(Validate(inpcType), "All injected properties must be public virtual read/write allowed");
            return (T)new ProxyGenerator().CreateClassProxy(inpcType, args, new InpcInterceptor());
        }

        public static T Decorate<T>(T target) where T : class, INotifyPropertyChangedCaller
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            var inpcType = typeof(T);
            Debug.Assert(Validate(inpcType), "All injected properties must be public virtual read/write allowed");
            return (T)new ProxyGenerator().CreateClassProxyWithTarget(inpcType, target, new InpcInterceptor());
        }

        internal static bool Validate(Type inpcType)
        {
            foreach (var prop in inpcType.GetProperties())
            {
                if (prop.IsDefined(typeof(InpcAttribute), true) && !(prop.GetGetMethod()?.IsVirtual == true && prop.GetSetMethod()?.IsVirtual == true))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
