using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokApp.Models
{
    public class StokBarang
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Barang")]
        public int BarangId { get; set; }
        public Barang Barang { get; set; } = null!;

        [Required]
        public int TotalStok { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}