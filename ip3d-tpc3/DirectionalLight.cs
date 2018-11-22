using Microsoft.Xna.Framework;

namespace ip3d_tpc3
{
    class DirectionalLight
    {

        public Vector4 Direction;

        public Vector4 Color;

        public float Intensity;

        public DirectionalLight(Vector4 direction, Vector4 color, float intensity)
        {

            Direction = direction;

            Color = color;

            Intensity = intensity;

        }

    }
}
