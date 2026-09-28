using System;

namespace MantenimientoLibros.Models
{
    public class Libro
    {
        public string TitleId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? PubId { get; set; }
        public string? PubName { get; set; }
        public string? City { get; set; }
        public decimal? Price { get; set; }
        public decimal? Advance { get; set; }
        public int? Royalty { get; set; }
        public int? YtdSales { get; set; }
        public string? Notes { get; set; }
        public DateTime? PubDate { get; set; }
    }
}