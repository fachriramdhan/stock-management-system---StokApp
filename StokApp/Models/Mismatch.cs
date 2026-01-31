using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class Mismatch
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Barang")]
        public int BarangId { get; set; }
        public Barang Barang { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TujuanAwal { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TujuanAktual { get; set; } = string.Empty;

        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        [ForeignKey("Area")]
        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;

        [Required]
        public int Jumlah { get; set; }

        [Required]
        public DateTime WaktuKejadian { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Catatan { get; set; }
    }
}