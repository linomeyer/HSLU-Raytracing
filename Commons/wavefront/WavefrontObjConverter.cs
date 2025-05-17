using Commons._2D;
using Commons._3D;
using Commons.Materials;

namespace Commons.wavefront;

public class WavefrontObjConverter(string filepath, Material material, Vector3D coordinates, double scale)
{
    private string Filepath => filepath;
    private Material Material => material;
    private Vector3D Coordinates => coordinates;
    private double Scale => scale;

    public List<Triangle> ConvertWavefrontObjToTriangle()
    {
        using (var streamreader = new StreamReader(filepath))
        {
            var wavefront = new WavefrontObj();
            string? line;
            while ((line = streamreader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                var splitLine = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (splitLine[0].ToUpper())
                {
                    case "V":
                        if (splitLine.Length >= 4 &&
                            double.TryParse(splitLine[1], out var x) &&
                            double.TryParse(splitLine[2], out var y) &&
                            double.TryParse(splitLine[3], out var z))
                            wavefront.Vertices.Add(new Vector3D(x, y, z));
                        break;
                    case "vn":
                        if (splitLine.Length >= 4 &&
                            double.TryParse(splitLine[1], out var nx) &&
                            double.TryParse(splitLine[2], out var ny) &&
                            double.TryParse(splitLine[3], out var nz))
                            wavefront.Normals.Add(new Vector3D(nx, ny, nz).Normalize());
                        break;
                    case "vt":
                        if (splitLine.Length >= 3 &&
                            double.TryParse(splitLine[1], out var u) &&
                            double.TryParse(splitLine[2], out var v))
                            wavefront.TextureCoordinates.Add(new Vector2D(u, v));
                        break;
                }
            }
        }
    }
}