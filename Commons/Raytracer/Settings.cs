namespace Commons;

public record Settings(string OutputFilename, int Depth = 6, bool DoMultithreading = false);

public record Dimensions(int Width, int Height);