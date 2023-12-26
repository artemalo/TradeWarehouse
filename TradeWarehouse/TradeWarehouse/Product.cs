using System;
using System.Text;

namespace TradeWarehouse
{
    internal class Product : FileWorker, IComparable<Product>
    {
        string name;
        Ulid article;
        double price;
        uint countInventory;
        uint countCurrent;

        public static ushort GetLengthArgs { get => 5; }

        public static void Report (DateTime d1, DateTime d2)
        {
            //........................................
        }

        public Product(string name, Ulid article, double price, uint countInventory, uint countCurrent)
        {
            this.name = name;
            this.article = article == null ? Ulid.NewUlid() : article;
            this.price = price;
            this.countInventory = countInventory;
            this.countCurrent = countCurrent;
        }
        public Product()
        {
            name = "-";
            article = Ulid.NewUlid();
            price = 0;
            countInventory = 0;
            countCurrent = 0;
        }
        ~Product()
        {
            Console.WriteLine($"~ Product: {name}");
        }
        public string Name { get => name; set => name = value; }
        public Ulid Article { get => article; set { article = value == null ? Ulid.NewUlid() : value; } }
        public double Price { get => price; set => price = value; }
        public uint CountInventory { get => countInventory; set => countInventory = value; }
        public uint CountCurrent { get => countCurrent; set => countCurrent = value; }

        int IComparable<Product>.CompareTo(Product other)
        {
            if (other is null) throw new ArgumentException("Некорректное значение параметра");
            return article.CompareTo(other.article);
        }

        protected override bool FillFromLine(string[] parts)
        {
            if (parts.Length == GetLengthArgs && Ulid.TryParse(parts[1], out article)) {
                name = parts[0];
                price = double.Parse(parts[2]);
                countInventory = uint.Parse(parts[3]);
                countCurrent = uint.Parse(parts[4]);
                return true;
            }
            else
                return false;
        }
        public override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(name).Append(' ').Append(article.ToString()).Append(' ').Append(price).Append(' ').Append(countInventory).Append(' ').Append(countCurrent);
        }

    }
}