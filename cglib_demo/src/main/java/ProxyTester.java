import net.sf.cglib.proxy.Enhancer;
import net.sf.cglib.proxy.MethodInterceptor;

public class ProxyTester {

    public static void main(String args[]) {
        RealClass proxy = (RealClass) createProxy(RealClass.class, new AddOneInterceptor());
        int i = 5;
        int j = 10;
        System.out.println(String.format("%s + %s = %s, but proxy is %s", i, j, i + j, proxy.Add(i, j)));
    }


    public static Object createProxy(Class clazz, MethodInterceptor interceptor) {
        try {
            Enhancer e = new Enhancer();
            e.setSuperclass(clazz);
            e.setCallback(interceptor);
            Object bean = e.create();
            return bean;
        } catch (Throwable e) {
            e.printStackTrace();
            throw new Error(e.getMessage());
        }

    }
}
