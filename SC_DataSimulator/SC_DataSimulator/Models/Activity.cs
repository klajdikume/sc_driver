using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SC_DataSimulator.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Id")]
        public int DriverId { get; set; }
        public DateTime Date { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
