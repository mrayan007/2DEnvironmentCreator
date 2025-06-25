using System.ComponentModel.DataAnnotations;

namespace EnvCreatorApi.DTOs
{
    public class EnvironmentDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "MaxHeight is required.")]
        public double MaxHeight { get; set; }
        [Required(ErrorMessage = "MaxWidth is required.")]
        public double MaxWidth{ get; set; }
    }
}
