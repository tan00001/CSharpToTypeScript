using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TestAssembly
{
    [DataContract]
    public class Person
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1024)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Title { get; set; }
    }
}