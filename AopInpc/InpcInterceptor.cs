using Castle.DynamicProxy;

namespace AopInpc
{
    public class InpcInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (invocation.Method.Name.StartsWith("set_"))
            {
                var propertyName = invocation.Method.Name.Substring(4);
                var propertyInfo = invocation.TargetType.GetProperty(propertyName);

                if (propertyInfo.IsDefined(typeof(InjectInpcAttribute), false))
                {
                    var proxy = invocation.Proxy as INotifyPropertyChangedCaller;
                    proxy.RaisePropertyChanged(propertyName);
                }
            }
        }
    }
}
