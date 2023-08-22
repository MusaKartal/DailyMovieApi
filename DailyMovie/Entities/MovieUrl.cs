using System.ComponentModel.DataAnnotations;

namespace DailyMovie.Entities
{
    public class MovieUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PullDate { get; set; }

        [Required]
        [StringLength(500)]
        [Url]
        public string Url { get; set; }

        [StringLength(200)]
        public string MovieId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ProcessingDate { get; set; }

        public bool IsProcessed { get; set; }
    }
}
