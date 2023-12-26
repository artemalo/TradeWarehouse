using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TradeWarehouse
{
    /// <summary>Работа с файлом</summary>
    public abstract class FileWorker
    {
        #region Paths
        public static readonly string pProduct = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\Products.txt";

        public static readonly string pAcceptanceActsHeaders = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\AcceptanceActs\Headers.txt";
        public static readonly string pAcceptanceActsLines = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\AcceptanceActs\Lines.txt";

        public static readonly string pDocumentsHeaders = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\Documents\Headers.txt";
        public static readonly string pDocumentsLines = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\Documents\Lines.txt";

        public static readonly string pCatalogSupplier = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\Catalog\Suppliers.txt";
        public static readonly string pCatalogDelivery = @"D:\GitHub\TradeWarehouse\TradeWarehouse\TradeWarehouse\Catalog\Deliverers.txt";
        #endregion

        /// <summary>Записывает все корректные строки(объекты) из файла в спикок.</summary>
        /// <exception cref="Exception"></exception>
        public static void ReadFileToList<T>(string path, List<T> items) where T : FileWorker, new()
        {
            if (File.Exists(path))
                using (StreamReader file = new StreamReader(path, Encoding.GetEncoding(1251)))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        T newItem = new T();
                        if (newItem.FillFromLine(parts))
                            items.Add(newItem);
                        else
                            Console.WriteLine($"Некорректные данные в строке: <{line}>, <{path}>. Неверное количество аргументов или неверный формат строки.");
                    }
                }
            else
                throw new Exception("Не найден путь " + path);
            GC.Collect();
        }
        
        /// <summary>Записывает/Перезаписывает объект в файл</summary>
        /// <param name="append"> true добавляние в конец файла, false - перезаписывание файла</param>
        /// <exception cref="Exception"></exception>
        public static void WriteObjectToFile<T>(string path, bool append, T obj) where T : FileWorker
        {
            if (File.Exists(path) && obj != null)
            {
                bool isNewLine = IsNewLine(path);
                using (StreamWriter file = new StreamWriter(path, append, Encoding.GetEncoding(1251)))
                {
                    if (!isNewLine && append)
                        file.WriteLine();
                    file.WriteLine(obj.StringBuild().ToString());
                }
                if (append)
                    Console.WriteLine($"+ Объект был добавлен в <{path}>");
                else
                    Console.WriteLine($"+ Объект был перезаписан в <{path}>");
            }
            else
                throw new Exception($"Не найден файл <{path}>. Или объект null.");
        }

        /// <summary>Записывает/Перезаписывает список в файл</summary>
        /// <param name="append"> true добавляние в конец файла, false - перезаписывание файла</param>
        /// <exception cref="Exception"></exception>
        public static void WriteListToFile<T>(string path, bool append, List<T> items) where T: FileWorker
        {
            if (File.Exists(path))
            {
                bool isNewLine = IsNewLine(path);
                using (StreamWriter file = new StreamWriter(path, append, Encoding.GetEncoding(1251)))
                {
                    if (!isNewLine && append)
                        file.WriteLine();
                    for (int i = 0; i < items.Count; ++i)
                        file.WriteLine(items[i].StringBuild().ToString());
                }
                if (append)
                    Console.WriteLine($"+ Список был добавлен в <{path}>");
                else
                    Console.WriteLine($"+ Список был перезаписан в <{path}>");
            }
            else
                throw new Exception("Не найден путь " + path);
        }

        private static bool IsNewLine(string path)
        {
            using (StreamReader lastChar = new StreamReader(path, Encoding.GetEncoding(1251)))
            {
                string content = lastChar.ReadToEnd();
                return string.IsNullOrEmpty(content) || content.Last() == '\n';
            }    
        }

        protected abstract bool FillFromLine(string[] parts);
        public abstract StringBuilder StringBuild();
    }
}
