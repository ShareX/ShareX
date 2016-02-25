using System.Drawing;

namespace ShareX.HelpersLib
{
    public struct CIELab
    {
        public double L { get; set; }

        public double A { get; set; }

        public double B { get; set; }

        public CIELab(double l, double a, double b) : this()
        {
            L = l;
            A = a;
            B = b;
        }

        public static implicit operator CIELab(Color color)
        {
            return ColorHelpers.ColorToCIELab(color);
        }

        public static implicit operator Color(CIELab color)
        {
            return color.ToColor();
        }

        public static implicit operator RGBA(CIELab color)
        {
            return color.ToColor();
        }

        public static implicit operator HSB(CIELab color)
        {
            return color.ToColor();
        }

        public static implicit operator CMYK(CIELab color)
        {
            return color.ToColor();
        }

        public static bool operator ==(CIELab item1, CIELab item2)
        {
            return (
                item1.L == item2.L
                && item1.A == item2.A
                && item1.B == item2.B
                );
        }

        public static bool operator !=(CIELab item1, CIELab item2)
        {
            return (
                item1.L != item2.L
                || item1.A != item2.A
                || item1.B != item2.B
                );
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}", L, A, B);
        }

        public Color ToColor()
        {
            return ColorHelpers.CIELabToColor(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}