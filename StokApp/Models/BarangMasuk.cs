using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class BarangMasuk
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Barang wajib dipilih")]
        [ForeignKey("Barang")]
        public int BarangId { get; set; }
        public Barang Barang { get; set; } = null!;

        [StringLength(200)]
        public string AsalBarang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tujuan awal wajib dipilih")]
        [StringLength(100)]
        public string TujuanAwal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant wajib dipilih")]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        [StringLength(200)]
        public string LokasiPenyimpanan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jumlah wajib diisi")]
        public int Jumlah { get; set; }

        [Required(ErrorMessage = "Tanggal wajib diisi")]
        public DateTime Tanggal { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Keterangan { get; set; }
    }
}