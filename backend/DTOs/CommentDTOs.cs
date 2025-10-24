namespace Optiviera.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class CreateCommentRequest
    {
        public string Note { get; set; } = string.Empty;
    }

    public class UpdateCommentRequest
    {
        public string? Note { get; set; }
    }
}
