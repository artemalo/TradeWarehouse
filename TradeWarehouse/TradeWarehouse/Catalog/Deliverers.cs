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
