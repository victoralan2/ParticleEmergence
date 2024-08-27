public struct ParticleInfo {
    public float posX;
    public float posY;
    public float velX;
    public float velY;
    public uint color;
}

public struct Params {
    public uint numParticles;
    public int xCells;
    public int yCells;
    public float delta;
    public float maxRadius;
    public int numColors;
    public float timeScale;
}
public struct CellInfo {
    public int startIndex;
    public uint numberOfParticles;
};