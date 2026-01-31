using System.ComponentModel.DataAnnotations;

namespace StokApp.Models
{
    public class Plant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nama { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Deskripsi { get; set; }

        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}