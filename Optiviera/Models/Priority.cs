using System.ComponentModel.DataAnnotations;

namespace Optiviera.Models
{
    public class Priority
    {
        public int Id { get; set; }

        [Display(Name = "Priority Name")]
        public string Name { get; set; }
    }
}
