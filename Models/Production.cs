namespace StageCraft.Models
{
    public class Production
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Playwright { get; set; } = string.Empty;
        public int Runtime { get; set; }
        public DateTime OpeningDay { get; set; }
        public DateTime ClosingDay { get; set; }
        public DateTime ShowTimeEve { get; set; }
        public int Season { get; set; }
        public bool IsWorldPremiere { get; set; }
        public string TicketLink { get; set; } = string.Empty;
        public string PosterImagePath { get; set; } = string.Empty;
    }
}
