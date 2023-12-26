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
                List<Product> listProduct = new List<Product>();
                FileWorker.ReadFileToList(FileWorker.pProduct, listProduct);
                foreach (Product product in listProduct)
                    Console.WriteLine(product.Article.ToString());

                Console.WriteLine("List<Product> = " + listProduct.Count.ToString());

                Console.WriteLine("========");


                /*List<AcceptanceActs.Lines> listLines = new List<AcceptanceActs.Lines>
                {
                    new AcceptanceActs.Lines("Товар3", Ulid.Parse("01HJ4FEMAMEVXXC0GAWXSKJA5J"), "шт", 9.5, 24.1, 100),
                    new AcceptanceActs.Lines("Товар4", Ulid.Parse("01HJ4FEMAMC9P4SPWKAYP2ZR4N"), "шт", 90.49, 100.59, 1902)
                };
                AcceptanceActs.Headers.Register(header: new AcceptanceActs.Headers(supplier: "[Александр Черкесск]"), listLines: listLines);

                Console.WriteLine("========");
                List<Documents.Lines> listDocLines = new List<Documents.Lines>
                {
                    new Documents.Lines(price: 10.1, count: 15),
                    new Documents.Lines(price: 20.2, count: 20),
                    new Documents.Lines(price: 30.3, count: 25),
                    new Documents.Lines(price: 40.4, count: 30)
                };
                Documents.Headers.Register(new Documents.Headers(deliverer: "[Артем Чекрекесск,Ленина,1]"), listDocLines);*/
                AcceptanceActs.Headers.PrintHeadersLinesToFile(@"C:\Users\sant6\Desktop\AcceptanceActs.txt");
                Documents.Headers.PrintHeadersLinesToFile(@"C:\Users\sant6\Desktop\Documents.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine("<!>-" + ex.Message);
            }
        }
    }
}
