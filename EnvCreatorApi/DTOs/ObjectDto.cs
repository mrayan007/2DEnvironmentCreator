using System.ComponentModel.DataAnnotations;

public class ObjectDto
{
    [Required]
    public int EnvironmentId { get; set; }
    [Required]
    public string PrefabId { get; set; }
    [Required]
    public double PositionX { get; set; }
    [Required]
    public double PositionY { get; set; }
    [Required]
    public double ScaleX { get; set; }
    [Required]
    public double ScaleY { get; set; }
    [Required]
    public double RotationZ { get; set; }
    [Required]
    public int SortingLayer { get; set; }
}