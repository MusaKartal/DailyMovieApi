using System.ComponentModel.DataAnnotations;

namespace DailyMovie.Entities
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie title is required.")]
        [StringLength(100, ErrorMessage = "Movie title can have a maximum of 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        [StringLength(300, ErrorMessage = "Image can have a maximum of 300 characters.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Year information is required.")]
        [Range(1900, 2100, ErrorMessage = "Enter a valid year.")]
        public string Date { get; set; }

        [Required(ErrorMessage = "Genre information is required.")]
        [StringLength(50, ErrorMessage = "Genre can have a maximum of 50 characters.")]
        public string FullDate { get; set; }

        [StringLength(500, ErrorMessage = "Description can have a maximum of 500 characters.")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PullDate { get; set; }

        [Required]
        [StringLength(500)]
        [Url]
        public string Url { get; set; }

        [StringLength(200)]
        public string MovieId { get; set; }
     
        public string? Score { get; set; }

        public string? Trailer { get; set; }

        public string? Time { get; set; }

        public string? Genres { get; set; }
      
        public bool IsViewed { get; set; }
    }
}
