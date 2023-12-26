using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TradeWarehouse.Catalog;

namespace TradeWarehouse.Documents
{
    internal class Headers : FileWorker, IComparable<Headers>
    {
        uint number;
        DateTime date;
        Deliverers deliverer;
        double amount;

        public static ushort GetLengthArgs { get => 5; }

        /// <summary> Добавление в таблицу один Заголовок и несколько записей Строк </summary>
        public static void Register(Headers header, List<Lines> listLines)
        {
            if (header == null || listLines == null) return;
            #region Auto number
            List<Headers> fileListHeader = new List<Headers>();
            ReadFileToList(pDocumentsHeaders, fileListHeader);
            fileListHeader?.Sort();
            header.number = fileListHeader.Count == 0 ? 0 : ++fileListHeader[fileListHeader.Count - 1].number;
            #endregion

            header.date = DateTime.Now;

            #region Deliverers - from fileListDeliverers, else add header.deliverer in this list
            List<Deliverers> fileListDeliverers = new List<Deliverers>();
            ReadFileToList(pCatalogDelivery, fileListDeliverers);
            bool foundMatch = false;
            foreach (Deliverers deliverer in fileListDeliverers)
                if (deliverer.Name == header.deliverer.Name && deliverer.Address == header.deliverer.Address)
                {
                    foundMatch = true;
                    break;
                }
            if (!foundMatch)
                WriteObjectToFile(pCatalogDelivery, true, header.deliverer);
            #endregion

            List<Product> fileListProducts = new List<Product>();
            ReadFileToList(pProduct, fileListProducts);
            SetArticulesForLines(listLines, fileListProducts);

            //Для каждого элемента в списке присваивает его номер
            //+ общ сумму фактуры += цене Строки
            foreach (Lines lines in listLines)
            {
                lines.Number = header.number;
                header.amount += lines.Price * lines.Count;
            }


            //По каждой записи фактуры корректируется текущее количество в "Карточке"
            foreach (Lines lines in listLines)
                foreach (Product product in fileListProducts)
                    if (lines.Article == product.Article)
                    {
                        product.CountCurrent -= lines.Count;
                        break;
                    }
            WriteListToFile(pProduct, false, fileListProducts);

            WriteObjectToFile(pDocumentsHeaders, true, header);
            WriteListToFile(pDocumentsLines, true, listLines);

            fileListHeader.Clear();
            fileListDeliverers.Clear();
            fileListProducts.Clear();
        }

        private static void SetArticulesForLines(List<Lines> listLines, List<Product> fileListProducts)
        {
            int listCount = listLines.Count;
            Console.WriteLine($"Выберете {listCount.ToString()} артикул/а/ов из списка продуктов:");
            foreach (Product product in fileListProducts)
            {
                Console.WriteLine($"\t{product.Name} {product.Article.ToString()} текущее количество: {product.CountCurrent.ToString()}");
            }

            Console.WriteLine("(Значение не должно превышать текущее количество товара с отпускаемым количеством)");
            int c = 0;//Кол-во удаленных элементов списка listLines
            for (int i = 0; i < listCount; ++i)
            {
                Console.Write($"Отпускаемое количество = {listLines[i - c].Count.ToString()}, Артикул: ");
                string articule = Console.ReadLine();
                Ulid ulid;
                int countCurrentProduct = -1;
                while (!Ulid.TryParse(articule, out ulid) ||
                    (countCurrentProduct = Contains(ulid, fileListProducts)) == -1)
                {
                    Console.Write("Не верный формат или уже есть в списке продуктов, попробуйте снова: ");
                    articule = Console.ReadLine();
                }
                if (listLines[i - c].Count <= countCurrentProduct)
                {
                    listLines[i - c].Article = ulid;
                    Console.WriteLine($"\t{(i + 1).ToString()} : {articule} добавлен");
                }
                else
                {
                    listLines.RemoveAt(i - c);
                    ++c;
                    Console.WriteLine($"\t{(i + 1).ToString()} : {articule} не добавлен");
                }
            }
        }

        /// <summary>Печать Фактуры в указанный файл из баз данных Headers и Lines</summary>
        /// <param name="path">Перезапись файла. Если файл остутсвует, он будет создан</param>
        public static void PrintHeadersLinesToFile(string path)
        {
            List<Headers> fileListHeaders = new List<Headers>();
            ReadFileToList(pDocumentsHeaders, fileListHeaders);
            List<Lines> fileListLines = new List<Lines>();
            ReadFileToList(pDocumentsLines, fileListLines);

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
            Console.WriteLine("Фактура была напечатана");
        }

        private static int Contains(Ulid ulid, List<Product> fileListProducts)
        {
            int foundMatch = -1;
            foreach (var product in fileListProducts)
            {
                if (product.Article == ulid)
                {
                    foundMatch = (int)product.CountCurrent;
                    break;
                }
            }
            return foundMatch;
        }
        public Headers(string deliverer)
        {
            string[] parts = deliverer.Replace("[", string.Empty).Replace("]", string.Empty).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == Deliverers.GetLengthArgs)
                this.deliverer = new Deliverers(parts[0], parts[1]);
            else
                throw new Exception("Некорректное создание объекта. Возможно аргументов должно быть " + GetLengthArgs.ToString());
        }
        public Headers() { }
        ~Headers()
        {
            Console.WriteLine("~ Documents.Headers: " + number.ToString());
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
                && double.TryParse(parts[4], out amount))
            {
                
                deliverer = new Deliverers(parts[2].Replace("[", string.Empty), parts[3].Replace("]", string.Empty));
                return true;
            }
            else
                return false;
        }
        public override StringBuilder StringBuild()
        {
            return new StringBuilder().Append(number).Append(" ").Append(date.ToShortDateString()).Append(" [").Append(deliverer.Name).Append(" ").Append(deliverer.Address).Append("] ").Append(amount);
        }
    }
}
