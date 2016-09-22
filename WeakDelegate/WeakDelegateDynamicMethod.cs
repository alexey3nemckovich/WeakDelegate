using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace WeakDelegate
{
    
    class WeakDelegateDynamicMethod
    {

        private Type eventHandlerDelegateType;
        private MethodInfo targetEventHandlerMethodInfo;
        private WeakReference weakReferenceToTarget;
        private Delegate proxyDelegate;

        public WeakDelegateDynamicMethod(Delegate eventHandler)
        {
            eventHandlerDelegateType = eventHandler.GetType();
            targetEventHandlerMethodInfo = eventHandler.Method;
            weakReferenceToTarget = new WeakReference(eventHandler.Target);
            InitProxyDelegate();
        }

        private void InitProxyDelegate()
        {
            Type targetEventHandlerReturnType = targetEventHandlerMethodInfo.ReturnType;
            Type[] parametersTypes = GetParametersTypes(targetEventHandlerMethodInfo.GetParameters());

            Type[] allParameters = new Type[parametersTypes.Length + 1];
            Array.Copy(parametersTypes, 0, allParameters, 1, parametersTypes.Length);
            allParameters[0] = typeof(WeakDelegateDynamicMethod);

            DynamicMethod proxyEventHandler = new DynamicMethod(
                "ProxyEventHandler",
                targetEventHandlerReturnType,
                allParameters,
                typeof(WeakDelegateDynamicMethod)
            );

            ILGenerator ilGenerator = proxyEventHandler.GetILGenerator();

            Label endMethodLabel = ilGenerator.DefineLabel();
            
            var weakReferenceFieldInfo = this.GetType().GetField("weakReferenceToTarget", BindingFlags.NonPublic | BindingFlags.Instance);
            var targetPropertyGetMethodInfo = weakReferenceToTarget.GetType().GetProperty("Target").GetGetMethod();
            Type[] invokeMethodArgs = new Type[] { typeof(object), typeof(object[]) };
            var invokeMethodInfo = targetEventHandlerMethodInfo.GetType().GetMethod("Invoke", invokeMethodArgs);

            ilGenerator.Emit(OpCodes.Nop);

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, weakReferenceFieldInfo);
            ilGenerator.Emit(OpCodes.Callvirt, targetPropertyGetMethodInfo);
            ilGenerator.Emit(OpCodes.Stloc_0);

            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldnull);
            ilGenerator.Emit(OpCodes.Cgt_Un);
            ilGenerator.Emit(OpCodes.Stloc_1);

            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Brfalse_S, endMethodLabel);

            ilGenerator.Emit(OpCodes.Nop);

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, targetEventHandlerMethodInfo);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Callvirt, invokeMethodInfo);
            ilGenerator.Emit(OpCodes.Pop);

            ilGenerator.Emit(OpCodes.Nop);

            ilGenerator.MarkLabel(endMethodLabel);
            ilGenerator.Emit(OpCodes.Ret);

            Type[] delegateParameters = new Type[parametersTypes.Length+1];
            Array.Copy(parametersTypes, 0, delegateParameters, 0, parametersTypes.Length);
            delegateParameters[delegateParameters.Length - 1] = targetEventHandlerReturnType;
            Type delegateType = Expression.GetDelegateType(delegateParameters);

            proxyDelegate = proxyEventHandler.CreateDelegate(delegateType, this);
        }

        private Type[] GetParametersTypes(ParameterInfo[] paramsInfo)
        {
            Type[] paramsTypes = new Type[paramsInfo.Length];
            for(int i = 0; i < paramsInfo.Length; i++)
            {
                paramsTypes[i] = paramsInfo[i].ParameterType;
            }
            return paramsTypes;
        }

        public Delegate Weak
        {
            get
            {
                return proxyDelegate;
            }
        }

        public void EventHandler(params object[] args)
        {
            Object target = weakReferenceToTarget.Target;
            if(target != null)
            {
                targetEventHandlerMethodInfo.Invoke(target, args);
            }
        }

    }

}