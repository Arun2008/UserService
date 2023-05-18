using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserService.Models
{

    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
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
