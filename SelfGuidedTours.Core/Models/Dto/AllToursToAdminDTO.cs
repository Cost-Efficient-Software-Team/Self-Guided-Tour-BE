namespace SelfGuidedTours.Core.Models.Dto
{
    public class AllToursToAdminDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string CreatorName { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
    }
}
