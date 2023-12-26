using System;
using System.Collections.Generic;
using System.Text;

namespace TradeWarehouse.AcceptanceActs
{
    internal class Lines : FileWorker, IComparable<Lines>
    {
        uint number;
        string nameProduct;
        Ulid article;
        string unit;
        double inputPrice;
        double outputPrice;
        uint countProduct;

        public static ushort GetLengthArgs { get => 7; }
        public static double TotalInputAmount(uint number, List<Lines> lines)
        {
            double amount = 0;
            bool flag = false;
            for (int i = 0; i < lines.Count; ++i)
                if (lines[i].number == number)
                {
                    amount += lines[i].inputPrice * lines[i].countProduct;
                    flag = true;
                }

            return flag ? amount : -1;
        }
        public static double TotalOutputAmount(uint number, List<Lines> lines)
        {
            double amount = 0;
            bool flag = false;
            for (int i = 0; i < lines.Count; ++i)
                if (lines[i].number == number)
                {
                    amount += lines[i].outputPrice * lines[i].countProduct;
                    flag = true;
                }

            return flag ? amount : -1;
        }

        public Lines() {}
        public Lines(string nameProduct, Ulid article, string unit, double inputPrice, double outputPrice, uint countProduct)
        {
            this.nameProduct = nameProduct;
            this.article = article;
            this.unit = unit;
            this.inputPrice = inputPrice;
            this.outputPrice = outputPrice;
            this.countProduct = countProduct;
        }

        public string NameProduct { get => nameProduct; set => nameProduct = value; }
        public uint Number { get => number; set => number = value; }
        public Ulid Article { get => article; }
        public double InputPrice { get => inputPrice; set => inputPrice = value; }
        public double OutputPrice { get => outputPrice; set => outputPrice = value; }
        public uint CountProduct { get => countProduct; set => countProduct = value; }

        int IComparable<Lines>.CompareTo(Lines other)
        {
            if (other is null) throw new ArgumentException("Некорректное значение параметра");
            return number.CompareTo(other.number);
        }

        protected override bool FillFromLine(string[] parts)
        {
            if (parts.Length == GetLengthArgs
                && uint.TryParse(parts[0], out number)
                && Ulid.TryParse(parts[2], out article)
                && double.TryParse(parts[4], out inputPrice)
                && double.TryParse(parts[5], out outputPrice)
                && uint.TryParse(parts[6], out countProduct))
            {
                nameProduct = parts[1];
                unit = parts[3];
                return true;
            }
            else
                return false;
        }
        public override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(number).Append(" ").Append(nameProduct).Append(" ").Append(article.ToString()).Append(" ").Append(unit).Append(" ").Append(inputPrice).Append(" ").Append(outputPrice).Append(" ").Append(countProduct);
        }
    }
}
