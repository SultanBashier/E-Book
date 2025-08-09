using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bulkyBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display order")]
        [Range(1, 100, ErrorMessage = "The Display Order Must Between 1 to 100")]
        public int Displayorder { get; set; }

        
    }
}
