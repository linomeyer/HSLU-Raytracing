using Commons._2D;
using Commons._3D;
using Commons.Materials;

namespace Commons.wavefront;

public class WavefrontObjConverter(string filepath, Material material, Vector3D position, double scale)
{
    private readonly WavefrontObj _wavefront = new();

    /**
     * uses only extracted indices (case "f" -> ParseCoordinatesToTriangles) to create our triangles but can be easily modified to return the whole Wavefront file input.
     */
    public List<Triangle> ConvertWavefrontObjToTriangle()
    {
        using var streamreader = new StreamReader(filepath);
        // is { } evaluates to true if value is not null
        while (streamreader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#')) continue;

            var splitLine = line.Split(['\t', ' '], StringSplitOptions.RemoveEmptyEntries);

            switch (splitLine[0].ToUpper())
            {
                case "V":
                    ParseVertex(splitLine);
                    break;
                case "vn":
                    ParseNormal(splitLine);
                    break;
                case "vt":
                    ParseTexture(splitLine);
                    break;
                case "f":
                    ParseCoordinatesToTriangles(splitLine);
                    break;
            }
        }

        return ScaleAndMoveTriangles();
    }

    private List<Triangle> ScaleAndMoveTriangles()
    {
        return _wavefront.Triangles.Select(triangle =>
            new Triangle(
                triangle.A * scale + position,
                triangle.B * scale + position,
                triangle.C * scale + position,
                material)
        ).ToList();
    }

    private void ParseCoordinatesToTriangles(string[] splitLine)
    {
        if (splitLine.Length >= 4)
        {
            List<Vector3D> vectors = [];
            for (var i = 1; i < splitLine.Length; i++) vectors.Add(ParseIndicesToVector(splitLine[i - 1]));

            // if 4 vectors create 2 triangles with index 0,1,2 and 0,2,3
            for (var i = 1; i < vectors.Count - 1; i++) _wavefront.Triangles.Add(new Triangle(vectors[0], vectors[i], vectors[i + 1], material));
        }
    }

    /**
     * converts entries like these "3089/3104/223" to Vector3D
     */
    private static Vector3D ParseIndicesToVector(string indexesString)
    {
        var vectorValues = indexesString.Split("/").Select(indexString => int.TryParse(indexesString, out var parsedIndex) ? parsedIndex : -1).ToArray();
        return new Vector3D(vectorValues[0], vectorValues[1], vectorValues[2]);
    }

    private void ParseTexture(string[] splitLine)
    {
        if (splitLine.Length >= 3 &&
            double.TryParse(splitLine[1], out var u) &&
            double.TryParse(splitLine[2], out var v))
            _wavefront.TextureCoordinates.Add(new Vector2D(u, v));
    }

    private void ParseNormal(string[] splitLine)
    {
        if (splitLine.Length >= 4 &&
            double.TryParse(splitLine[1], out var nx) &&
            double.TryParse(splitLine[2], out var ny) &&
            double.TryParse(splitLine[3], out var nz))
            _wavefront.Normals.Add(new Vector3D(nx, ny, nz).Normalize());
    }

    private void ParseVertex(string[] splitLine)
    {
        if (splitLine.Length >= 4 &&
            double.TryParse(splitLine[1], out var x) &&
            double.TryParse(splitLine[2], out var y) &&
            double.TryParse(splitLine[3], out var z))
            _wavefront.Vertices.Add(new Vector3D(x, y, z));
    }
}