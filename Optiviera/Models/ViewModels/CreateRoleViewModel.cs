using System.ComponentModel.DataAnnotations;

namespace Optiviera.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set;}
    }
}
