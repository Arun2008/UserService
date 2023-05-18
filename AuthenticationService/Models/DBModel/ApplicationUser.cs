using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationService.Models.DBModel
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "nvarchar(100)")]
        public string? Name { get; set; } = string.Empty;
        public Guid? CreatorUserId { get; set; } = null;
        public DateTimeOffset? CreationDate { get; set; }
        public Guid? LastModifierUserId { get; set; } = null;
        public DateTimeOffset? LastModifyDate { get; set; } = null;
        public Guid? DeleterUserId { get; set; } = null;
        public DateTimeOffset? DeletionDate { get; set; } = null;
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }


}
