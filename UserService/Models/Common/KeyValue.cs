namespace UserService.Models.Common
{


    public class DropDownList
    {
        public List<KeyValue> Options { get; set; } = new();
    }
    public class KeyValue
    {
        public Guid Key { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    public class ActionController
    {
        public string? key { get; set; }
        public string? value { get; set; }
    }
    public class Ids
    {
        public Guid Id { get; set; }
    }
}
