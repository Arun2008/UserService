using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.DBModel
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; }
        public string? ModuleName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }

    }
}
