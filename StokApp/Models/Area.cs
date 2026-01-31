using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class Area
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama area wajib diisi")]
        [StringLength(100)]
        public string Nama { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Deskripsi { get; set; }

        [Required(ErrorMessage = "Plant wajib dipilih")]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        
        public Plant Plant { get; set; } = null!;
    }
}