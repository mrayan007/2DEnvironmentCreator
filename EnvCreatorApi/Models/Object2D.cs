using EnvCreatorApi.Models;

public class Object2D
{
    public int Id { get; set; }
    public string PrefabId { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double ScaleX { get; set; }
    public double ScaleY { get; set; }
    public double RotationZ { get; set; }
    public int SortingLayer { get; set; }
    public int EnvironmentId { get; set; }
    public Environment2D Environment { get; set; }
}