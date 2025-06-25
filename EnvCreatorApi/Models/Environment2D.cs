namespace EnvCreatorApi.Models
{
    public class Environment2D
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MaxHeight {  get; set; }
        public double MaxWidth { get; set; }
        public string UserId { get; set; }
        public List<Object2D> Objects { get; set; }
    }
}
