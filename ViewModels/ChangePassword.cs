using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_CS5.ViewModels
{
	public class ChangePassword
	{
		[Required(ErrorMessage = "Please enter password!")]
		[Column(TypeName = "varchar(50)"), MaxLength(50), MinLength(8, ErrorMessage = "This password too short")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
		
		[Required(ErrorMessage = "Please enter new password!")]
		[Column(TypeName = "varchar(50)"), MaxLength(50), MinLength(8, ErrorMessage = "This password too short")]
		[DataType(DataType.Password)]
		[Display(Name = "NewPassword")]
		public string NewPassword { get; set; }
		
		[Column(TypeName = "varchar(50)"), MaxLength(50)]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "Confirm password not match!")]
		[Display(Name = "Confirm new password")]
		public string ConfirmNewPassword { get; set; }
	}
}
