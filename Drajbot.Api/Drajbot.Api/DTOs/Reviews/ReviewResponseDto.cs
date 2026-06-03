namespace Drajbot.Api.DTOs.Reviews
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty; // Samo ime da ne otkrivamo email
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsVerifiedPurchase { get; set; } // Opciono, samo za proizvode
        public DateTime CreatedAt { get; set; }
    }
}