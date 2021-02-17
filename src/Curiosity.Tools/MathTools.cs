using System;

namespace Curiosity.Tools
{
    public static class MathTools
    {
        public static double Round(double value, int digits)
        {
            double scale = Math.Pow(10.0, digits);
            double round = Math.Floor(Math.Abs(value) * scale + 0.5);
            return (Math.Sign(value) * round / scale);
        }

        public static double Floor(double value, int digits)
        {
            double scale = Math.Pow(10.0, digits);
            double round = Math.Floor(Math.Abs(value) * scale);
            return (Math.Sign(value) * round / scale);
        }

        public static double Ceiling(double value, int digits)
        {
            double scale = Math.Pow(10.0, digits);
            double round = Math.Ceiling(Math.Abs(value) * scale);
            return (Math.Sign(value) * round / scale);
        }

        public static decimal Round(decimal value, int digits)
        {
            decimal scale = new decimal(Math.Pow(10.0, digits));
            decimal round = Math.Floor(Math.Abs(value) * scale + 0.5m);
            return (Math.Sign(value) * round / scale);
        }

        public static decimal Floor(decimal value, int digits)
        {
            decimal scale = new decimal(Math.Pow(10.0, digits));
            decimal round = Math.Floor(Math.Abs(value) * scale);
            return (Math.Sign(value) * round / scale);
        }

        public static decimal Ceiling(decimal value, int digits)
        {
            decimal scale = new decimal(Math.Pow(10.0, digits));
            decimal round = Math.Ceiling(Math.Abs(value) * scale);
            return (Math.Sign(value) * round / scale);
        }
    }
}