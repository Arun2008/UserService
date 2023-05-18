using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Models.Common;

namespace UserService.Models.DBModel
{

    public class ApplicationUserRole : IdentityRole
    {
        [Column(TypeName = "nvarchar(50)")]
        public Status Status { get; set; }
    }
}
