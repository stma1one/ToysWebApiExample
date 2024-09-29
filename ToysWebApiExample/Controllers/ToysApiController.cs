using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;
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
        ILogger<ToyRepository> logger;

        public ToysApiController(ToyRepository toyRepo, UserRepository userRepo, ILogger<ToyRepository> logger)
        {
            this.logger = logger;
            this.toyRepo = toyRepo;
            this.userRepo = userRepo;
        }
        // POST api/Register
        [HttpPost("Register")]
        public IActionResult Register(UserDTO user)
        {
            User newUser=new User() { Email=user.Email, Name=user.Name, Password=user.Password};
            if(!userRepo.AddUser(newUser))
            return Conflict("Unable to Add user");    
            return Ok(true);
        }
        // POST api/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginInfo? loginDto)
        {
            try
            {
                HttpContext.Session?.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 

                var user = userRepo.Login(loginDto.Email, loginDto.Password);

                //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
                if (user == null || user.Password != loginDto.Password)
                {
                    return Unauthorized("Login Failed UserName or Password Incorrect");
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session?.SetString("loggedInUser", user.Email);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Get api/toys
        [HttpGet(@"Toys/{typeId}")]
        public IActionResult GetToysByType(int typeId=0)
        {
            var user = HttpContext.Session.GetString("loggedInUser");
            if (user == null)
                return Unauthorized("You must first LOGIN to the Application");
            var result = toyRepo.GetToyByType(typeId);
            if (result.Count == 0)
                return NoContent();
            return Ok(result);
        }

        // POST api/AddToy
        [HttpPost("Toys")]
        public IActionResult AddToy([FromBody] Toy toy)
        {
            var user = HttpContext.Session.GetString("loggedInUser");
            if (user == null)
                return Unauthorized("You must first LOGIN to the Application");
            try
            {
                if (!toyRepo.AddToy(toy))
                    return BadRequest("Unable to Add Toy");

                return Ok(toy);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
         // Delete api/AddToy
        [HttpDelete("Toys/{toyId}")]
        public IActionResult DeleteToy(int toyId)
        {
            var user = HttpContext.Session.GetString("loggedInUser");
            if (user == null)
                return Unauthorized("You must first LOGIN to the Application");
            try
            {
                if (!toyRepo.DeleteToy(toyId))
                    return BadRequest("Unable to Delete toy");
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        //Put Service
        [HttpPut("Toys/Image/{toyId}")]
        public async Task<IActionResult> UploadToyImageAsync(int toyId,IFormFile photo)
        {
            // Define allowed file extensions for images
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            // Specify the directory where images will be saved
            string uploadDirectory = Path.Combine("wwwroot", @$"Images/ToyImages/{toyId}");
            // Check if a file was actually uploaded
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Get the file extension and convert to lowercase for consistency
            var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            // Validate file extension against our allowed list
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only jpg, jpeg, and png are allowed.");
            }
           //create directory if not exists
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }
            // Construct the file name using the specified format
            var fileName = $"Toy_{toyId}{fileExtension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Save the file to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
            string fullPath= @$"{Environment.GetEnvironmentVariable("VS_TUNNEL_URL")}/Images/ToyImages/{toyId}/{fileName}";
            toyRepo.UpdateImage(toyId, fullPath);

            return Ok();
        }
        //Get Types
        [HttpGet("ToyTypes")]
        public IActionResult GetToyTypes()
        {
             logger.LogInformation($"Get Toys{DateTime.Now}", DateTime.Now);
            return Ok(toyRepo.GetToyTypes());
        }
    }
}
