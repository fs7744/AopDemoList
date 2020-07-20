using System.Reflection;

namespace EmitAopDemo
{
    public abstract class MethodInterceptor
    {
        public abstract object Invoke(object instance, MethodInfo methodInfo, object[] parameters, object returnValue);
    }
}