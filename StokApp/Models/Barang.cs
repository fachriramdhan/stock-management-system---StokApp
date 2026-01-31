using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class Barang
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama barang wajib diisi")]
        [StringLength(200)]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori wajib dipilih")]
        [ForeignKey("Kategori")]
        public int KategoriId { get; set; }
        
        public Kategori Kategori { get; set; } = null!;

        [Required(ErrorMessage = "Satuan wajib dipilih")]
        [StringLength(50)]
        public string Satuan { get; set; } = "pcs";

        [Required(ErrorMessage = "Minimum stok wajib diisi")]
        public int MinimumStok { get; set; } = 0;

        [StringLength(1000)]
        public string? Keterangan { get; set; }

        public StokBarang? StokBarang { get; set; }
    }
}