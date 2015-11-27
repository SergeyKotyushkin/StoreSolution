using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreSolution.BusinessLogic.Database.Models
{
    [Table("PersonTable")]
    public class Person
    {
        [Key]
        public string Login { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }

        [Column(TypeName = "image")]
        [MaxLength]
        public byte[] Icon { get; set; }
    }
}
