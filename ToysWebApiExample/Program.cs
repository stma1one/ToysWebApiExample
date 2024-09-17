
using ToysWebApiExample.Repository;

namespace ToysWebApiExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            #region Add Database context to Dependency Injection
            //Read connection string from app settings.json
           /* string connectionString = builder.Configuration.GetSection("LoginDbConnectionString").Value;*/
            //Add Database to dependency injection
            builder.Services.AddSingleton<ToyRepository>();
            builder.Services.AddSingleton<UserRepository>();

            #endregion 
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            #region Add Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(300);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                
            });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            #region Add Session
            app.UseSession(); //In order to enable session management
            #endregion 
            app.UseHttpsRedirection();

            app.UseAuthorization();

            #region Use Static Files wwwroot folder
            app.UseStaticFiles();
            #endregion
            app.MapControllers();

            app.Run();
        }
    }
}
