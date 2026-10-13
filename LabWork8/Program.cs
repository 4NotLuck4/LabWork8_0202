using OrderManagementApp;
class Program
{
    static void Main(string[] args)
    {
        using var dbContext = new AppDbContext();
        dbContext.Database.EnsureCreated();

        var customerService = new CustomerService(dbContext);
        var orderService = new OrderService(dbContext);
        var priceCalculator = new OrderPriceCalculator();

        var customer = new Customer { Name = "Alice", Email = "alice@example.com" };
        customerService.AddCustomer(customer);

        var order = new Order { Total = 12000, IsExpress = true, Customer = customer };
        orderService.AddOrder(order);

        Console.WriteLine("--- Информация о клиенте ---");
        customerService.PrintCustomerInfo(customer.Id);

        Console.WriteLine("\n--- Детали заказа ---");
        orderService.PrintOrderDetails(order.Id);

        Console.WriteLine("\n--- Итоговая стоимость (со скидкой и НДС) ---");
        Console.WriteLine("Final Price: " + priceCalculator.Calculate(order));

        Console.ReadLine(); // Чтобы консоль не закрывалась сразу
    }
}
