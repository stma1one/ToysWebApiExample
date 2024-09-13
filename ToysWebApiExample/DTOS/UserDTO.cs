using System.Text.Json.Serialization;
using ToysWebApiExample.Models;

namespace ToysWebApiExample.DTOS
{
    public class UserDTO
    {
        public int Id
        {
            get; set;
        }
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

        public UserDTO()
        {
        }
        public UserDTO(User u)
        {
            this.Id = u.Id;
            Name = u.Name;
            Email = u.Email;
            Password = u.Password;
        }
    }
}
