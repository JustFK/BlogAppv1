namespace BlogApp.Entity
{
    public class Comment
    {
        public int CommentId { get; set; }

        public string? CommentText { get; set; } 

        public DateTime PublishedOn { get; set; }

        //Bu yorum hangi posta yapıldı. Unique bir yorum bir posta yapılır.
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        //bu yorumu yapan kim?
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
