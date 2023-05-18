using System.Text.Json.Serialization;

namespace AuthenticationService.Models
{
    public class CurrentUser
    {
        public Guid EmployeeId { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid OrgId { get; set; }
        public string UserName { get; set; } = "system";
    }
}
