using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using TradeWarehouse.Catalog;
using TradeWarehouse.Documents;

namespace TradeWarehouse.AcceptanceActs
{
    internal class Headers : FileWorker, IComparable<Headers>
    {
        uint number;
        DateTime date;
        Suppliers supplier;
        double totalInputAmount;
        double totalOutputAmount;

        public static ushort GetLengthArgs { get => 6; }

        /// <summary>Добавление в таблицу один Заголовок и несколько записей Строк</summary>
        public static void Register(Headers header, List<Lines> listLines)
        {
            if (header == null || listLines == null) return;
            #region Auto number
            List<Headers> fileListHeader = new List<Headers>();
            ReadFileToList(pAcceptanceActsHeaders, fileListHeader);
            fileListHeader?.Sort();
            header.number = fileListHeader.Count == 0 ? 0 : ++fileListHeader[fileListHeader.Count - 1].number;
            #endregion

            header.date = DateTime.Now;

            #region Supplier - from fileListSuppliers, else add header.supplier in this list
            List<Suppliers> fileListSuppliers = new List<Suppliers>();
            ReadFileToList(pCatalogSupplier, fileListSuppliers);
            bool foundMatch = false;
            foreach (var item in fileListSuppliers)
                if (item.Name == header.supplier.Name && item.Address == header.supplier.Address)
                {
                    foundMatch = true;
                    break;
                }
            if (!foundMatch) WriteObjectToFile(pCatalogSupplier, true, header.supplier);
            #endregion

            //Для каждого элемента в списке присваивает его номер
            for (int i = 0; i < listLines.Count; ++i)
                listLines[i].Number = header.number;

            List<Lines> fileListLines = new List<Lines>();
            ReadFileToList(pAcceptanceActsLines, fileListLines);

            //Добавление новых элементов в список
            fileListLines.AddRange(listLines);

            header.totalInputAmount = Lines.TotalInputAmount(header.number, fileListLines);
            header.totalOutputAmount = Lines.TotalOutputAmount(header.number, fileListLines);

            #region Product article and price are compared with lines, else add to Product. Overwrites Product
            List<Product> fileListProducts = new List<Product>();
            ReadFileToList(pProduct, fileListProducts);

            foreach (var item in listLines)
            {
                int index = -1;
                foundMatch = false;
                for (int j = 0; j < fileListProducts.Count; ++j)
                    if (fileListProducts[j].Article == item.Article)
                    {
                        if (fileListProducts[j].Price == item.OutputPrice)
                        {
                            fileListProducts[j].CountCurrent = item.CountProduct;
                            foundMatch = true;
                        }
                        index = j;
                    }
                Product itemProduct = new Product(item.NameProduct, item.Article, item.OutputPrice, item.CountProduct, item.CountProduct);
                if (!foundMatch && index != -1)
                    fileListProducts[index] = itemProduct;
                else
                    if (!foundMatch && index == -1)
                    fileListProducts.Add(itemProduct);
            }
            WriteListToFile(pProduct, false, fileListProducts);
            #endregion

            WriteObjectToFile(pAcceptanceActsHeaders, true, header);
            WriteListToFile(pAcceptanceActsLines, true, listLines);

            fileListHeader.Clear();
            fileListLines.Clear();
            fileListProducts.Clear();
            fileListSuppliers.Clear();
        }

        /// <summary>Печать Приемного акта в указанный файл из баз данных Headers и Lines</summary>
        /// <param name="path">Перезапись файла. Если файл остутсвует, он будет создан</param>
        public static void PrintHeadersLinesToFile(string path)
        {
            List<Headers> fileListHeaders = new List<Headers>();
            ReadFileToList(pAcceptanceActsHeaders, fileListHeaders);
            List<Lines> fileListLines = new List<Lines>();
            ReadFileToList(pAcceptanceActsLines, fileListLines);

            fileListHeaders.Sort();
            fileListLines.Sort();
            using (StreamWriter fileWriter = new StreamWriter(path, false, Encoding.GetEncoding(1251)))
                foreach (Headers header in fileListHeaders)
                {
                    fileWriter.WriteLine(header.StringBuild());
                    for (int i = 0; i < fileListLines.Count; ++i)
                        if (header.number == fileListLines[i].Number)
                        {
                            fileWriter.WriteLine(fileListLines[i].StringBuild().Insert(0, '\t'));
                            if (i != fileListLines.Count - 1 && fileListLines[i].Number != fileListLines[i + 1].Number)
                                break;
                        }
                    fileWriter.WriteLine();
                }
            Console.WriteLine("Приемный акт был напечатан");
        }

        public Headers(string supplier)
        {
            string[] parts = supplier.Replace("[", string.Empty).Replace("]", string.Empty).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == Suppliers.GetLengthArgs)
                this.supplier = new Suppliers(parts[0], parts[1]);
            else
                throw new Exception("Некорректное создание объекта. Возможно аргументов должно быть " + GetLengthArgs.ToString());
        }
        public Headers() {}
        ~Headers()
        {
            Console.WriteLine("~ AcceptanceActs.Headers: "+number.ToString());
        }

        int IComparable<Headers>.CompareTo(Headers other)
        {
            if (other is null) throw new ArgumentException("Некорректное значение параметра");
            return number.CompareTo(other.number);
        }

        protected override bool FillFromLine(string[] parts)
        {
            if (parts.Length == GetLengthArgs
                && uint.TryParse(parts[0], out number)
                && DateTime.TryParse(parts[1], out date)
                && double.TryParse(parts[4], out totalInputAmount)
                && double.TryParse(parts[5], out totalOutputAmount))
            {
                supplier = new Suppliers(parts[2].Replace("[", string.Empty), parts[3].Replace("]", string.Empty));
                return true;
            }
            else
                return false;
        }
        public override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(number).Append(" ").Append(date.ToShortDateString()).Append(" [").Append(supplier.Name).Append(" ").Append(supplier.Address).Append("] ").Append(totalInputAmount).Append(" ").Append(totalOutputAmount);
        }
    }
}