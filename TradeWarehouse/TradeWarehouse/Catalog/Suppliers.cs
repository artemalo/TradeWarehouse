using System;
using System.Collections.Generic;
using System.Text;

namespace TradeWarehouse.Catalog
{
    internal class Suppliers : FileWorker
    {
        string name;
        string address;
        public static ushort GetLengthArgs { get => 2; }

        public Suppliers() { }
        public Suppliers(string name, string address)
        {
            this.name = name;
            this.address = address;
        }

        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => name = value; }

        public void Add(Suppliers supplier)
        {
            WriteObjectToFile(pCatalogSuppliers, true, supplier);
            Console.WriteLine($"Поставщик [{supplier.StringBuild()}] добавлен в базу.");
        }

        public void Replace(Suppliers supplier, Suppliers other)
        {
            List<Suppliers> fileList = new List<Suppliers>();
            ReadFileToList(pCatalogSuppliers, fileList);

            bool notFoundMatch = true;
            for (int i = 0; i < fileList.Count && notFoundMatch; ++i)
            {
                if (fileList[i] == supplier)
                {
                    fileList[i].name = other.name;
                    fileList[i].address = other.address;
                    notFoundMatch = false;
                }
            }

            if (notFoundMatch)
                Console.WriteLine($"Не удалось найти поставщика [{other.StringBuild()}] в базе.");
            else
            {
                WriteListToFile(pCatalogSuppliers, false, fileList);
                Console.WriteLine($"Поставщик [{supplier.StringBuild()}] изменен на [{other.StringBuild()}].");
            }
        }

        public void Delete(Suppliers supplier)
        {
            List<Suppliers> fileList = new List<Suppliers>();
            ReadFileToList(pCatalogSuppliers, fileList);

            bool notFoundMatch = true;
            for (int i = 0; i < fileList.Count && notFoundMatch; ++i)
            {
                if (fileList[i] == supplier)
                {
                    fileList.RemoveAt(i);
                    notFoundMatch = false;
                }
            }

            if (notFoundMatch)
                Console.WriteLine($"Не удалось найти поставщика [{supplier.StringBuild()}] в базе.");
            else
            {
                WriteListToFile(pCatalogSuppliers, false, fileList);
                Console.WriteLine($"Поставщик [{supplier.StringBuild()}] удален из базы.");
            }
        }

        public static bool operator ==(Suppliers sup1, Suppliers sup2)
        {
            return sup1.name == sup2.name && sup1.address == sup2.address;
        }

        public static bool operator !=(Suppliers sup1, Suppliers sup2)
        {
            return !(sup1 == sup2);
        }

        public override bool Equals(object obj)
        {
            return obj is Suppliers suppliers &&
                   name == suppliers.name &&
                   address == suppliers.address;
        }

        public override int GetHashCode()
        {
            int hashCode = -99900638;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(address);
            return hashCode;
        }

        protected override bool FillFromLine(string[] parts)
        {
            if (parts.Length == GetLengthArgs) {
                name = parts[0];
                address = parts[1];
                return true;
            }
            else
                return false;
        }
        public override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(name).Append(" ").Append(address);
        }
    }
}
