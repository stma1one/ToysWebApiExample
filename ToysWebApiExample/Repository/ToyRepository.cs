using ToysWebApiExample.Models;

namespace ToysWebApiExample.Repository
{
    public class ToyRepository
    {
        static int id=0;
        static int user_id=0;
        List<User> users;
        List<Toy> toys;
        List<ToyTypes> toyTypes;
        public ToyRepository()
        {
            InitUsers();
            InitToyTypes();
            InitToys();
            
        }

        private void InitUsers()
        {
            users=new List<User>();
            users.Add(
                new() { Id = ++user_id, Email = "tals@gmail.com", Name = "Tal", Password = "1234" }
                );
        }

        private void InitToyTypes()
        {
            toyTypes = new List<ToyTypes>()
        {
            new ToyTypes()
            {
                Id = 1, Name = "פאזל"
            },

            new ToyTypes()
            {
            Id = 2, Name = "משחקי חשיבה"
            },
            new ToyTypes()
            {
            Id = 3, Name = "בובה"
            }
        };
        }

        private void InitToys()
        {
            toys = new List<Toy>()
            {
                new Toy()
                {
                    Id=++id,
                    Image="chuky.jpg",
                    IsSecondHand=false,
                    Name="צאקי",
                    Price=200,
                    Type=toyTypes[2]
                },
                new Toy()
                {
                    Id=++id,
                    Image="puppet.jpeg",
                    IsSecondHand=false,
                    Name="רובי",
                    Price=250,
                    Type=toyTypes[2]
                },
                new Toy()
                {
                    Id=++id,
                    Image="puzzle.jpeg",
                    IsSecondHand=false,
                    Name="גן חיות",
                    Price=250,
                    Type=toyTypes[0]
                },
                new Toy()
                {
                    Id=++id,
                    Image="thinkgame.jpeg",
                    IsSecondHand=true,
                    Name="מבוכים ודרקונים",
                    Price=250,
                    Type=toyTypes[1]
                }

            };
        }


        public List<Toy>? GetToys()
        {
            return toys.ToList();
        }

        public List<Toy> GetToyByType(int typeId=0)
        {
            if (typeId == 0)
                return toys.ToList();
            #region By LINQ
            //   return  toys.Where(t=>t.Type.Id==type.Id).ToList();
            #endregion
            List<Toy> result = new();
            
            foreach (var t in toys)
            {
                if (t.Type.Id == typeId)
                    result.Add(t);
            }
            return result;
        }


        public List<Toy>? GetToysByPriceCondition(double price, bool abovePrice)
        {
            List<Toy> result = new();
            foreach (var t in toys)
            {
                if (abovePrice)
                {
                    if (t.Price > price)
                        result.Add(t);
                }
                else
                    if (t.Price <= price)
                    result.Add(t);
            }
            return result;
        }

        #region Filter By delegate
        public List<Toy>? GetToysByPriceCondition(Predicate<double> condition)
        {
            return toys?.Where(x => condition(x.Price)).ToList();
        }
        #endregion

        public bool AddToy(Toy toy)
        {
            if (toys != null)
            {
                toy.Id = ++id;
                toy.Image = "default.png";
                toys.Add(toy);
                return true;
            }
            return false;

        }

        public bool DeleteToy(int toyId)
        {
            var t = toys.Find(t => t.Id == toyId);
                return toys.Remove(t);
        
             #region using LINQ
            // return toys.Remove(toys.Find((x)=>x.Id==toy.Id));
            #endregion
        }

        public Toy? GetToy(int id)
        {
        
        return toys.Find(x=>x.Id==id);
        }

        public bool UpdateImage(int toyId, string imageName)
        {
        var toy= toys.Find(x => x.Id == id);
            if(toy != null)
                toy.Image= imageName;
        return true;
        }

        public List<ToyTypes> GetToyTypes()
        {
            return toyTypes;
        }
        
    }
}
