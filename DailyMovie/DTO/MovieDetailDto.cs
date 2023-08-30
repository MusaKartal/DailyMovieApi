using System.ComponentModel.DataAnnotations;

namespace DailyMovie.DTO
{
    public class MovieDetailDto
    {      
        public int Id { get; set; }
    
        public string Title { get; set; }

        public string Image { get; set; }
     
        public string Date { get; set; }
        
        public string FullDate { get; set; }
      
        public string Description { get; set; }
   
        public string? Score { get; set; }

        public string? Trailer { get; set; }

        public string? Time { get; set; }

        public string? Genres { get; set; }
     
    }
}
