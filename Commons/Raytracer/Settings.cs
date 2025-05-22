namespace Commons.Raytracer;

public record Settings(string OutputFilename, int Depth = 6, bool DoMultithreading = false, int Threads = 12);

public record Dimensions(int Width, int Height);