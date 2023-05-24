using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Assignment_CS5.Models
{
    public enum Category
    {
        [Display(Name = "Thức ăn")]
        Food = 1,
        [Display(Name = "Thức uống")]
        Drink = 2,
        [Display(Name = "Combo")]
        Combo = 3
    }
    public class Menu
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Please enter a name!")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a price!"),
            Range(0,double.MaxValue,ErrorMessage = " Invalid price!")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Please choose category!"),
            Range(1, int.MaxValue, ErrorMessage = "Invalid classification!")]
        [Display(Name = "Category")]
        public Category Category { get; set; }

        [StringLength(100)]
        [Display(Name = "Image")]
        public string? Image { get; set; }

        [NotMapped]
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)"), MaxLength(255)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }
    }
}
