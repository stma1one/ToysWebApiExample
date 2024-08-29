using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ToysWebApiExample.Models
{
    public class User
    {
        public int Id {get;set; }
        public string? Name { get;set; }
        public string? Email { get;set; }
        [JsonIgnore]
        public string? Password { get;set; }
            

    }
}
