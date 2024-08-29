using ToysWebApiExample.Models;

namespace ToysWebApiExample.Repository;

    public class UserRepository
    {
        static int user_id = 0;
        List<User> users;

        public UserRepository() 
        {
            InitUsers();
        }
        private void InitUsers()
        {
            users = new List<User>();
            users.Add(
                new()
                {
                    Id = ++user_id,
                    Email = "tals@gmail.com",
                    Name = "Tal",
                    Password = "1234"
                }
                );
        }
    public User Login(string? userName, string? password)
    {
        return users.Where(u => u.Email == userName && u.Password == password).FirstOrDefault();
    }

    public bool AddUser(User user)
    {
        if (users.Any(u => u.Email == user.Email))
            return false;
        user.Id = ++user_id;
        users.Add(user);
        return true;    
    }

}
