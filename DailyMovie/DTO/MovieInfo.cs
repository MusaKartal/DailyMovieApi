namespace DailyMovie.DTO
{
    public class MovieInfo
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string FullReleaseDate { get; set; }
        public string Duration { get; set; }
        public string Score { get; set; }
        public string TrailerId { get; set; }
        public string History { get; set; }

        public List<string> Genres { get; } = new List<string>();
    }
}