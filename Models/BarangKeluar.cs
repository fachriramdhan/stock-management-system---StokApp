using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class BarangKeluar
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Barang wajib dipilih")]
        [ForeignKey("Barang")]
        public int BarangId { get; set; }
        public Barang Barang { get; set; } = null!;

        [Required(ErrorMessage = "Tujuan penggunaan wajib dipilih")]
        [StringLength(100)]
        public string TujuanPenggunaan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant wajib dipilih")]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        [Required(ErrorMessage = "Area wajib dipilih")]
        [ForeignKey("Area")]
        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;

        [Required(ErrorMessage = "Penanggung jawab wajib diisi")]
        [StringLength(200)]
        public string PenanggungJawab { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jumlah wajib diisi")]
        public int Jumlah { get; set; }

        [Required(ErrorMessage = "Tanggal wajib diisi")]
        public DateTime Tanggal { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Keterangan { get; set; }
    }
}