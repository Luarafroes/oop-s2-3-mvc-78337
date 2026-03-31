namespace VgcCollege.Domain.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int MaxScore { get; set; }
        public DateTime DueDate { get; set; }

        // Navigation
        public Course Course { get; set; } = null!;
        public ICollection<AssignmentResult> Results { get; set; } = new List<AssignmentResult>();
    }
}