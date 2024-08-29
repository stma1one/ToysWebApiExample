using System.Text.Json.Serialization;

namespace ToysWebApiExample.DTOS
{
    public class UserDTO
    {
        public string? Name
        {
            get; set;
        }
        public string? Email
        {
            get; set;
        }
        
        public string? Password
        {
            get; set;
        }
    }
}
