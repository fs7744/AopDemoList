namespace Aop.Roslyn.Jit
{
    public class RealClassProxy : RealClass 
    {
        public override int Add(int i, int j)
        {
            int r = default; 
            r = base.Add(i, j); 
            r++; 
            return r;
        }
    }
}