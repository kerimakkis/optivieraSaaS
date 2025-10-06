using Microsoft.AspNetCore.Mvc.Rendering;

namespace Optiviera.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public WaveUser User { get; set; }

        public MultiSelectList Roles { get; set; }

        public List<string> SelectedRoles { get; set; }
    }
}
