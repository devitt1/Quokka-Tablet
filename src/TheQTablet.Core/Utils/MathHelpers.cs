using System;
namespace TheQTablet.Core.Utils
{
    public static class MathHelpers
    {
        public static double ToRad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }
        public static float ToRadF(float deg)
        {
            return (float)ToRad(deg);
        }

        public static double ToDeg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }
        public static float ToDegF(float rad)
        {
            return (float)ToDeg(rad);
        }
    }
}
