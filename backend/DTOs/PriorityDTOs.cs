namespace Optiviera.DTOs
{
    public class PriorityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreatePriorityRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdatePriorityRequest
    {
        public string? Name { get; set; }
    }
}
