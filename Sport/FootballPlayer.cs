using System;

namespace Sport
{
    public class FootballPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int ShirtNumber { get; set; }

        public FootballPlayer(int id, string name, decimal price, int shirtNumber)
        {
            Id = id;
            if (name.Length < 4)
                throw new ArgumentOutOfRangeException("name", name.Length, "Name must be equal to or larger than 4 characters.");
            else
                Name = name;

            if (price <= 0)
                throw new ArgumentOutOfRangeException("price", price, "Price must be higher than 0");
            else
                Price = price;
            if (!(shirtNumber >= 1 && shirtNumber <= 100))
                throw new ArgumentOutOfRangeException("shirtNumber", shirtNumber, "Shirt Number must be between 1 and 100");
            else
                ShirtNumber = shirtNumber;

        }
    }
}
