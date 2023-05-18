namespace UserService.Models.Request
{
    public class VerifyPermission
    {
        public Guid RoleId { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
    }
}
