using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class Tools
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nama { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Jenis { get; set; } = string.Empty;

        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        [ForeignKey("Area")]
        public int? AreaId { get; set; }
        public Area? Area { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Available"; // Available, Dipakai, Rusak

        [StringLength(1000)]
        public string? Keterangan { get; set; }
    }
}