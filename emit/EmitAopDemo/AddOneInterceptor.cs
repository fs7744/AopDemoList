using System.Reflection;

namespace EmitAopDemo
{
    public class AddOneInterceptor : MethodInterceptor
    {
        public override object Invoke(object instance, MethodInfo methodInfo, object[] parameters, object returnValue)
        {
            return (int)returnValue + 1;
        }
    }
}