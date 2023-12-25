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
        protected override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(name).Append(" ").Append(address);
        }
    }
}
