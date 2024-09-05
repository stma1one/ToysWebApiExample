using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToysWebApiExample.DTOS;
using ToysWebApiExample.Models;
using ToysWebApiExample.Repository;
using static System.Net.Mime.MediaTypeNames;

namespace ToysWebApiExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToysApiController : ControllerBase
    {
        private ToyRepository toyRepo;
        private UserRepository userRepo;

        public ToysApiController(ToyRepository toyRepo, UserRepository userRepo)
        {
            this.toyRepo = toyRepo;
            this.userRepo = userRepo;
        }
        // POST api/Register
        [HttpPost("Register")]
        public ApiResponse<bool> Register(UserDTO user)
        {
            User newUser=new User() { Email=user.Email, Name=user.Name, Password=user.Password};
            if(!userRepo.AddUser(newUser))
            return ApiResponse<bool>.Error("Unable to Add user",(int)HttpStatusCode.Conflict);    
            return ApiResponse<bool>.Ok(true);
        }
        // POST api/login
        [HttpPost("login")]
        public ApiResponse<User> Login([FromBody] DTO.LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 

                var user = userRepo.Login(loginDto.Email, loginDto.Password);

                //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
                if (user == null || user.Password != loginDto.Password)
                {
                    return ApiResponse<User>.Error("Login Failed UserName or Password Incorrect", (int)HttpStatusCode.Unauthorized);
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", user.Email);

                return ApiResponse<User>.Ok(user);
            }
            catch (Exception ex)
            {
                return ApiResponse<User>.Error(ex.Message, (int)HttpStatusCode.BadRequest);
            }

        }

        // Get api/toys
        [HttpGet(@"Toys/{typeId}")]
        public ApiResponse<List<Toy>> GetToysByType(int typeId=0)
        {
            var user = HttpContext.Session.Get("loggedInUser");
            if (user == null)
                return ApiResponse<List<Toy>>.Error("You must first LOGIN to the Application", (int)HttpStatusCode.Unauthorized);
            return ApiResponse<List<Toy>>.Ok(toyRepo.GetToyByType(typeId));
        }

        // POST api/AddToy
        [HttpPost("Toys")]
        public ApiResponse<Toy> AddToy([FromBody] Toy toy)
        {
            var user = HttpContext.Session.Get("loggedInUser");
            if (user == null)
                return ApiResponse<Toy>.Error("You must first LOGIN to the Application", (int)HttpStatusCode.Unauthorized);
                
            if(!toyRepo.AddToy(toy))
                return ApiResponse<Toy>.Error("Unable to Add toy", (int)HttpStatusCode.NotModified);
            return ApiResponse<Toy>.Ok(toy);
        }
         // POST api/AddToy
        [HttpDelete("Toys/{toyId}")]
        public ApiResponse<Toy> DeleteToy(int toyId)
        {
            var user = HttpContext.Session.Get("loggedInUser");
            if (user == null)
                return ApiResponse<Toy>.Error("You must first LOGIN to the Application", (int)HttpStatusCode.Unauthorized);
                
            if(!toyRepo.DeleteToy(toyId))
                return ApiResponse<Toy>.Error("Unable to Delete toy", (int)HttpStatusCode.NoContent);
            return ApiResponse<Toy>.Ok();
        }
        [HttpPut("Toys/Image/{toyId}")]
        public async Task<ApiResponse<bool>> UploadToyImageAsync(int toyId,IFormFile photo)
        {
            // Define allowed file extensions for images
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            // Specify the directory where images will be saved
            string uploadDirectory = Path.Combine("wwwroot", "Images");
            // Check if a file was actually uploaded
            if (photo == null || photo.Length == 0)
            {
                return ApiResponse<bool>.Error("No file uploaded.",(int)HttpStatusCode.BadRequest );
            }

            // Get the file extension and convert to lowercase for consistency
            var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            // Validate file extension against our allowed list
            if (!allowedExtensions.Contains(fileExtension))
            {
                return ApiResponse<bool>.Error("Invalid file type. Only jpg, jpeg, and png are allowed.",(int)HttpStatusCode.BadRequest);
            }
           //create directory if not exists
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }
            // Construct the file name using the specified format
            var fileName = $"{toyId}product{fileExtension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Save the file to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
            toyRepo.UpdateImage(toyId, fileName);

            return ApiResponse<bool>.Ok();
        }

        public ApiResponse<List<ToyTypes>> GetToyTypes()
        {
            return ApiResponse<List<ToyTypes>>.Ok(toyRepo.GetToyTypes());
        }
    }
}
