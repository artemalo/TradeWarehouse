﻿using System;
using System.Text;

namespace TradeWarehouse.Documents
{
    internal class Lines : FileWorker, IComparable<Lines>
    {
        uint number;
        Ulid article;
        double price;
        uint count;

        public static ushort GetLengthArgs { get => 4; }

        public Lines() { }
        public Lines(double price, uint count)
        {
            this.price = price;
            this.count = count;
        }

        public uint Number { get => number; set => number = value; }
        public Ulid Article { get  => article; set => article = value; }
        public double Price { get  => price; set => price = value; }
        public uint Count { get => count; set => count = value; }

        int IComparable<Lines>.CompareTo(Lines other)
        {
            if (other is null) throw new ArgumentException("Некорректное значение параметра");
            return number.CompareTo(other.number);
        }

        protected override bool FillFromLine(string[] parts)
        {
            if (parts.Length == GetLengthArgs
                && uint.TryParse(parts[0], out number)
                && Ulid.TryParse(parts[1], out article)
                && double.TryParse(parts[2], out price)
                && uint.TryParse(parts[3], out count))
            {
                return true;
            }
            else
                return false;
        }
        public override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(number).Append(" ").Append(article.ToString()).Append(" ").Append(price).Append(" ").Append(count);
        }
    }
}
