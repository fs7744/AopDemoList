using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitAopDemo
{
    public static class ProxyGenerator
    {
        private static ModuleBuilder moduleBuilder;
        private static MethodInfo getMethodMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
        private static MethodInfo invoke = typeof(MethodInterceptor).GetMethod("Invoke");

        static ProxyGenerator()
        {
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EmitAopDemoTest"), AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = asmBuilder.DefineDynamicModule("Proxy");
        }

        public static T Generate<T>(Type methodInterceptorType)
        {
            var proxyType = GenerateProxyType(typeof(T), methodInterceptorType);
            return (T)Activator.CreateInstance(proxyType);
        }

        public static Type GenerateProxyType(Type type, Type methodInterceptorType)
        {
            var typeBuilder = moduleBuilder.DefineType($"{type.Name}Proxy", TypeAttributes.Class | TypeAttributes.Public, type);
            foreach (var m in type.GetTypeInfo().DeclaredMethods)
            {
                var ps = m.GetParameters().Select(i => i.ParameterType).ToArray();
                var newM = typeBuilder.DefineMethod(m.Name, MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, m.CallingConvention, m.ReturnType, ps);
                CreateProxyMethod(methodInterceptorType, m, ps, newM);
                typeBuilder.DefineMethodOverride(newM, m);
            }
            return typeBuilder.CreateType();
        }

        private static void CreateProxyMethod(Type methodInterceptorType, MethodInfo m, Type[] ps, MethodBuilder newM)
        {
            var il = newM.GetILGenerator();
            var argsLocal = il.DeclareLocal(typeof(object[]));
            var returnLocal = il.DeclareLocal(typeof(object));

            // 初始化参数集合
            il.Emit(OpCodes.Ldc_I4, ps.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            for (var i = 0; i < ps.Length; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Box, ps[i]);
                il.Emit(OpCodes.Stelem_Ref);
            }
            il.Emit(OpCodes.Stloc, argsLocal);

            // 调用被代理方法
            il.Emit(OpCodes.Ldarg, 0); // load this
            for (var i = 0; i < ps.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
            }
            il.Emit(OpCodes.Call, m);
            il.Emit(OpCodes.Box, newM.ReturnType);
            il.Emit(OpCodes.Stloc, returnLocal);

            //调用方法后拦截器
            il.Emit(OpCodes.Newobj, methodInterceptorType.GetConstructors().First());
            il.Emit(OpCodes.Ldarg, 0); // load this
            //加载方法信息参数
            il.Emit(OpCodes.Ldtoken, m);
            il.Emit(OpCodes.Call, getMethodMethod);
            il.Emit(OpCodes.Castclass, typeof(MethodInfo));

            il.Emit(OpCodes.Ldloc, argsLocal);
            il.Emit(OpCodes.Ldloc, returnLocal);
            il.Emit(OpCodes.Callvirt, invoke);
            il.Emit(OpCodes.Stloc, returnLocal);

            // return
            il.Emit(OpCodes.Ldloc, returnLocal);
            il.Emit(OpCodes.Unbox_Any, newM.ReturnType);
            il.Emit(OpCodes.Ret);
        }
    }
}