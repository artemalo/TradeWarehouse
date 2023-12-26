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

        public Suppliers() {}
        public Suppliers(string name, string address)
        {
            this.name = name;
            this.address = address;
        }

        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => name = value; }

        public void Add(Suppliers supplier)
        {
            WriteObjectToFile(pCatalogSupplier, true, supplier);
        }

        public void Replace(Suppliers supplier, Suppliers other)
        {
            List<Suppliers> fileList = new List<Suppliers>();
            ReadFileToList(pCatalogSupplier, fileList);
            
            foreach (Suppliers fileSuplier in fileList)
            {
                if (fileSuplier == other)
                {
                    //....
                }
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
