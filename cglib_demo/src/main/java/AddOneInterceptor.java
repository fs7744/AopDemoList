import net.sf.cglib.proxy.MethodInterceptor;
import net.sf.cglib.proxy.MethodProxy;

import java.lang.reflect.Method;

public class AddOneInterceptor implements MethodInterceptor {
    public Object intercept(Object o, Method method, Object[] objects, MethodProxy methodProxy) throws Throwable {
        if(method.getReturnType() == int.class) {
            return ((Integer) methodProxy.invokeSuper(o, objects)) + 1;
        } else {
            return methodProxy.invokeSuper(o, objects);
        }
    }
}
