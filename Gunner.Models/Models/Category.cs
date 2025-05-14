using System.ComponentModel.DataAnnotations;

namespace Gunner.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Range(1,100 , ErrorMessage ="Display Order Must be between 1-100")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }


    }
}
