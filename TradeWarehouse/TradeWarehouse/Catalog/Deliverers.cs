using System.Collections.Generic;
using System;
using System.Text;

namespace TradeWarehouse.Catalog
{
    internal class Deliverers : FileWorker
    {
        string name;
        string address;

        public static ushort GetLengthArgs { get => 2; }

        public Deliverers(string name, string address)
        {
            this.name = name;
            this.address = address;
        }
        public Deliverers() { }

        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => address = value; }

        public void Add(Deliverers deliverer)
        {
            WriteObjectToFile(pCatalogDeliverers, true, deliverer);
            Console.WriteLine($"Получатель [{deliverer.StringBuild()}] добавлен в базу.");
        }

        public void Replace(Deliverers deliverer, Deliverers other)
        {
            List<Deliverers> fileList = new List<Deliverers>();
            ReadFileToList(pCatalogDeliverers, fileList);

            bool notFoundMatch = true;
            for (int i = 0; i < fileList.Count && notFoundMatch; ++i)
            {
                if (fileList[i] == deliverer)
                {
                    fileList[i].name = other.name;
                    fileList[i].address = other.address;
                    notFoundMatch = false;
                }
            }

            if (notFoundMatch)
                Console.WriteLine($"Не удалось найти получателя [{other.StringBuild()}] в базе.");
            else
            {
                WriteListToFile(pCatalogDeliverers, false, fileList);
                Console.WriteLine($"Получатель [{deliverer.StringBuild()}] изменен на [{other.StringBuild()}].");
            }
        }

        public void Delete(Deliverers deliverer)
        {
            List<Deliverers> fileList = new List<Deliverers>();
            ReadFileToList(pCatalogDeliverers, fileList);

            bool notFoundMatch = true;
            for (int i = 0; i < fileList.Count && notFoundMatch; ++i)
            {
                if (fileList[i] == deliverer)
                {
                    fileList.RemoveAt(i);
                    notFoundMatch = false;
                }
            }

            if (notFoundMatch)
                Console.WriteLine($"Не удалось найти получателя [{deliverer.StringBuild()}] в базе.");
            else
            {
                WriteListToFile(pCatalogSuppliers, false, fileList);
                Console.WriteLine($"Получатель [{deliverer.StringBuild()}] удален из базы.");
            }
        }

        public static bool operator ==(Deliverers dev1, Deliverers dev2)
        {
            return dev1.name == dev2.name && dev1.address == dev2.address;
        }

        public static bool operator !=(Deliverers dev1, Deliverers dev2)
        {
            return !(dev1 == dev2);
        }

        public override bool Equals(object obj)
        {
            return obj is Deliverers deliverer&&
                   name == deliverer.name &&
                   address == deliverer.address;
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
