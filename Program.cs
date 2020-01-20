using System;
using System.Data.Common;
using System.Linq;


namespace EntityFramework
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var db = new NorthwindContext();

            do
            {
                switch (Menu())
                {
                    case 1:
                        Overzicht(db);
                        break;
                    case 2:
                        Analyse(db);
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Deze optie is niet gekent");
                        break;
                }
                Verder();
            } while (true);
        }

        private static void Analyse(NorthwindContext db)
        {
            var regions = from o in db.Orders
                join c in db.Customers on o.CustomerId equals c.CustomerId
                join d in db.OrderDetails on o.OrderId equals d.OrderId
                
                select new
                {
                    d.Quantity,
                    d.UnitPrice,
                    c.Country
                };
            Console.WriteLine("Land\t\tTotaal Verkocht");
            foreach(var region in regions)
            {
                Console.WriteLine("{0}\t{1}", region.Country,region.Quantity*region.UnitPrice);
            }
        }     

        private static void Overzicht(NorthwindContext db)
        {
            var orders = from o in db.Orders join c in db.Customers on o.CustomerId equals c.CustomerId select new
            {
                o.OrderId,
                o.CustomerId,
                o.Customer,
                o.Employee
            };
            int width = Console.WindowWidth;
            int pad = width / 8;
            Console.Write("Order ID".PadRight(pad)+"Klant ID".PadRight(pad)+"KlantNaam".PadRight(pad*4)+"Werknemer naam".PadRight(pad*2)+"\n");
            Console.Write(String.Concat(Enumerable.Repeat("-", width)));
            foreach (var order in orders)
            {
                Console.WriteLine(order.OrderId.ToString().PadRight(pad) +
                                  order.CustomerId.PadRight(pad) + 
                                  order.Customer.CompanyName.PadRight(pad * 4) +
                                  order.Employee.FirstName.PadRight(pad * 2));
            }
            int orderId;
            do
            {
                Console.Write("Geef een geldig order id in om detail info te bekijken: ");
                
                if (!int.TryParse(Console.ReadLine(), out orderId))
                {
                    Console.WriteLine("Onbekende input probeer opnieuw");
                }
            } while (orderId == 0);
            Console.WriteLine("Product ID".PadRight(pad)+"Productnaam".PadRight(pad*3)+"Aantal".PadRight(pad)+"Eenheidsprijs".PadRight(pad*2)+"LijnWaarde".PadRight(pad));
            Console.Write(String.Concat(Enumerable.Repeat("-", width)));
            var products =
                from o in db.OrderDetails
                where o.OrderId == orderId
                join p in db.Products
                on o.Product.ProductId equals p.ProductId
                select new
            {
                o.ProductId,
                p.ProductName,
                p.QuantityPerUnit,
                o.UnitPrice,
                p.ReorderLevel
            };
            foreach (var product in products)
            {
                Console.WriteLine(product.ProductId.ToString().PadRight(pad) +
                                  product.ProductName.PadRight(pad) +
                                  product.QuantityPerUnit.PadRight(pad * 3) +
                                  product.UnitPrice.ToString().PadRight(pad * 2)+
                                  product.ReorderLevel.ToString().PadRight(pad));
            }
        }
        
        

        private static void Verder()
        {
            Console.Write("Druk op een toetst om verder te gaan");
            Console.ReadKey();
            Console.Clear();
        }
        
        private static int Menu()
        {
            string str = "1. Overzicht klantbestellingen\n2. Data analyse\n3. Stop\n > ";
            Console.Write(str);
            int keuze = 0;
            int.TryParse(Console.ReadLine(), out keuze);
            return keuze;
        }
    }
}