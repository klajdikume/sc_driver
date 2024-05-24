using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace SC_DataSimulator.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Activity> Activities { get; } = new List<Activity>();
    }
}
