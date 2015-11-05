using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreSolution.DatabaseProject.Model
{
    [Table("PersonTable")]
    public class Person
    {//public int Id { get; set; }
        [Key]
        public string Login { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }

        [Column(TypeName = "image")]
        [MaxLength]
        public byte[] Icon { get; set; }
    }
}
