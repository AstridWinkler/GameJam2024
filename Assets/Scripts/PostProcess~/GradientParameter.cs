

namespace UnityEngine.Rendering.PostProcessing
{
    [System.Serializable]
    public sealed class GradientParameter : ParameterOverride<Gradient>
    {
        


        public GradientParameter()
        {
            value = new Gradient();
        }

        public override void Interp(Gradient from, Gradient to, float t)
        {
            if (t == 0)
                value  = from;
            else
            value = to;
        }
    }
}