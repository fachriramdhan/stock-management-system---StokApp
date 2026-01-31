using System.ComponentModel.DataAnnotations;

namespace StokApp.Models
{
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nama { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Deskripsi { get; set; }

        public ICollection<Barang> Barangs { get; set; } = new List<Barang>();
    }
}