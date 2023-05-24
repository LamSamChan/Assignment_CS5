using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Assignment_CS5.ViewModels
{
    public class ViewLogin
    {
         public string UserName { get; set;}
		[DataType(DataType.Password)]
		public string Password { get; set;}

    }
}
