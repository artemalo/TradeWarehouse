using System;
using System.Collections.Generic;

namespace TradeWarehouse
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                /*List<Product> listProduct = new List<Product>();
                FileWorker.ReadFileToList(FileWorker.pProduct, listProduct);
                foreach (Product product in listProduct)
                    Console.WriteLine(product.Article.ToString());

                Console.WriteLine("List<Product> = " + listProduct.Count.ToString());*/

                List<AcceptanceActs.Lines> listLines = new List<AcceptanceActs.Lines>
                {
                    new AcceptanceActs.Lines("Чипсы", Ulid.NewUlid(), "шт", 9.5, 74.1, 200),
                    new AcceptanceActs.Lines("Салат", Ulid.Parse("00000000000000000000000005"), "шт", 90.49, 100.59, 1000)
                };
                AcceptanceActs.Headers.Register(header: new AcceptanceActs.Headers(supplier: "[Алекс Город1]"), listLines: listLines);

                /*Console.WriteLine("========");
                List<Documents.Lines> listDocLines = new List<Documents.Lines>
                {
                    new Documents.Lines(price: 10.1, count: 15),
                    new Documents.Lines(price: 20.2, count: 20),
                    new Documents.Lines(price: 30.3, count: 25),
                    new Documents.Lines(price: 40.4, count: 30)
                };
                Documents.Headers.Register(new Documents.Headers(deliverer: "[Артем Чекрекесск,Ленина,1]"), listDocLines);

                AcceptanceActs.Headers.PrintHeadersLinesToFile(@"C:\Users\sant6\Desktop\AcceptanceActs.txt");
                Documents.Headers.PrintHeadersLinesToFile(@"C:\Users\sant6\Desktop\Documents.txt");

                Product.Report(@"C:\Users\sant6\Desktop\Report.txt", new DateTime(2023, 12, 19), new DateTime(2023, 12, 23));*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("<!>-" + ex.Message);
            }
        }
    }
}
