namespace Commons;

public class RgbColor(double r, double g, double b)
{
    public double B => b;
    public double G => g;
    public double R => r;

    public static RgbColor operator *(RgbColor a, RgbColor b) => new(a.R * b.R, a.G * b.G, a.B * b.B);
}