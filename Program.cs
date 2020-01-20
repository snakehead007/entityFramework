using System;
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
                        return;
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
                select new {aantal = d.UnitPrice * d.Quantity, country = c.Country};
            var regions_sum = from r in regions
                group r.aantal by r.country
                into groep
                select new
                {
                    country=groep.Key,aantal=groep.Sum()
                };
                 var width = Console.WindowWidth;
            var pad = width / 6;
            Console.WriteLine("Land".PadRight(pad) + "Totaal Verkocht".PadRight(pad));
            
            foreach (var region in regions_sum)
                Console.WriteLine("{0}".PadRight(pad) + "{1}", region.country,region.aantal);
            
            
            Console.Write("Geef de landnaam in om details te bekijken: ");
            string countryInQuestion = Console.ReadLine();
            Console.Write("Klant ID".PadRight(pad*2) + "KlantNaam".PadRight(pad * 2) +
                          "Werknemer naam".PadRight(pad * 2) + "\n");
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", width)));
            
            
            var regionDetail = from o in db.Orders
                join c in db.Customers on o.CustomerId equals c.CustomerId
                join d in db.OrderDetails on o.OrderId equals d.OrderId
                where c.Country == countryInQuestion
                select new {aantal = d.UnitPrice * d.Quantity, customerId = c.CustomerId};
            var regionDetail_sum = from r in regionDetail
                group r.aantal by r.customerId
                into groep
                select new
                {
                    customerId=groep.Key, aantal=groep.Sum()
                };
            var regionDetail_All = from s in regionDetail_sum
                join c in db.Customers on s.customerId equals c.CustomerId
                select new
                {
                    costumerId = c.CustomerId, 
                    name = c.ContactName,
                    aantal = s.aantal
                };
            foreach (var region in regionDetail_All)
            {
                Console.WriteLine("{0}".PadRight(pad*2)+"{1}".PadRight(pad*2)+"{2}".PadRight(pad*2), region.costumerId, region.name,region.aantal);
            }

            Console.WriteLine("Geef een klant id in: ");
            string costumerInQuestion = Console.ReadLine();
            var pad2 = width / 8;
            Console.Write("Order ID".PadRight(pad2*2) + "Order Datum".PadRight(pad2*2) + "verzenddatum".PadRight(pad2 * 2) +
                          "Totaal".PadRight(pad2 * 2) + "\n");
            Console.Write(string.Concat(Enumerable.Repeat("-", width)));
            
            
            
            var klant = from c in db.Customers
                join o in db.Orders on c.CustomerId equals o.CustomerId
                join d in db.OrderDetails on o.OrderId equals d.OrderId
                where c.CustomerId == costumerInQuestion
                select new {aantal = d.UnitPrice * d.Quantity, orderId = o.OrderId};
           
            var klantSum = from k in klant
                group k.aantal by k.orderId
                into groep
                select new
                {
                    orderId=groep.Key, 
                    aantal=groep.Sum()
                };
         
            var ordersDetail = from s in klantSum
                join o in db.Orders on s.orderId equals o.OrderId
                select new
                {
                    orderId = s.orderId,
                    orderDatum = o.OrderDate,
                    shippedDatum = o.ShippedDate,
                    aantal = s.aantal
                };
            foreach (var order in ordersDetail)
            {
                Console.WriteLine("{0}".PadRight(pad*2)+"{1}".PadRight(pad*2)+"{2}".PadRight(pad*2)+"{3}".PadRight(pad*2), order.orderId, order.orderDatum,order.shippedDatum,order.aantal);
            }

            Verder();
        }

        private static void Overzicht(NorthwindContext db)
        {
            var orders = from o in db.Orders
                join c in db.Customers on o.CustomerId equals c.CustomerId
                select new {o.OrderId, o.CustomerId, o.Customer, o.Employee};
            var width = Console.WindowWidth;
            var pad = width / 8;
            Console.Write("Order ID".PadRight(pad) + "Klant ID".PadRight(pad) + "KlantNaam".PadRight(pad * 4) +
                          "Werknemer naam".PadRight(pad * 2) + "\n");
            Console.Write(string.Concat(Enumerable.Repeat("-", width)));
            foreach (var order in orders)
                Console.WriteLine(order.OrderId.ToString().PadRight(pad) + order.CustomerId.PadRight(pad) +
                                  order.Customer.CompanyName.PadRight(pad * 4) +
                                  order.Employee.FirstName.PadRight(pad * 2));
            int orderId;
            do
            {
                Console.Write("Geef een geldig order id in om detail info te bekijken: ");
                if (!int.TryParse(Console.ReadLine(), out orderId))
                    Console.WriteLine("Onbekende input probeer opnieuw");
            } while (orderId == 0);

            Console.WriteLine("Product ID".PadRight(pad) + "Productnaam".PadRight(pad * 3) + "Aantal".PadRight(pad) +
                              "Eenheidsprijs".PadRight(pad * 2) + "LijnWaarde".PadRight(pad));
            Console.Write(string.Concat(Enumerable.Repeat("-", width)));
            var products = from o in db.OrderDetails
                where o.OrderId == orderId
                join p in db.Products on o.Product.ProductId equals p.ProductId
                select new
                {
                    o.ProductId,
                    p.ProductName,
                    p.QuantityPerUnit,
                    o.UnitPrice,
                    p.ReorderLevel
                };
            foreach (var product in products)
                Console.WriteLine(product.ProductId.ToString().PadRight(pad) + product.ProductName.PadRight(pad) +
                                  product.QuantityPerUnit.PadRight(pad * 3) +
                                  product.UnitPrice.ToString().PadRight(pad * 2) +
                                  product.ReorderLevel.ToString().PadRight(pad));
        }

        private static void Verder()
        {
            Console.Write("Druk op een toetst om verder te gaan");
            Console.ReadKey();
            Console.Clear();
        }

        private static int Menu()
        {
            var str = "1. Overzicht klantbestellingen\n2. Data analyse\n3. Stop\n > ";
            Console.Write(str);
            var keuze = 0;
            int.TryParse(Console.ReadLine(), out keuze);
            return keuze;
        }
    }
}