// Исходный код приложения для рефакторинга.
// Рефакторинг зафиксировать в текстовом документе со столбцами 
// Задание | Исходный код | Код после рефакторинга
// Разнести типы данных по разным файлам.

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace OrderManagementApp
{
    // Контекст базы данных
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = @"Server=mssql;Database=ispp3102;Trusted_Connection=True;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    // Task 4 / Инкопсулирование поля Name
    // Класс Customer (клиент)
    public class Customer
    {
        private string _name;
        public int Id { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Имя не может быть пустым или null");
                }
                _name = value;
            }
        }
        public string Email { get; set; }
        public List<Order> Orders { get; set; }

        // Task 5 / Перемещение метода вывода информации
        public void PrintEmail()
        {
            Console.WriteLine("Customer Email: " + Email);
        }
    }

    // Класс Order (заказ)
    public class Order
    {
        public int Id { get; set; }
        public double Total { get; set; }
        public bool IsExpress { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    // Сервис для работы с клиентами
    public class CustomerService
    {
        private readonly AppDbContext _dbContext;

        public CustomerService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddCustomer(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
        }

        public void PrintCustomerInfo(int customerId)
        {
            var customer = _dbContext.Customers.Include(c => c.Orders).FirstOrDefault(c => c.Id == customerId);

            // Task 7 / Замена вложеных условий / тут небыло проверки на null / добавлен guard clouses
            if (customer == null) return;

            Console.WriteLine("Customer: " + customer.Name);
            Console.WriteLine("Email: " + customer.Email);
        }
    }

    // Сервис для работы с заказами
    public class OrderService
    {
        private readonly AppDbContext _dbContext;

        public OrderService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddOrder(Order order)
        {
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
        }

        public void PrintOrderDetails(int orderId)
        {
            var order = _dbContext.Orders.Include(o => o.Customer).FirstOrDefault(o => o.Id == orderId);

            // Task 6 / Вызов извлеченных методов
            PrintOrderId(order);
            PrintTotal(order);
            PrintExpressShipping(order);
            PrintCustomerEmail(order);

        }

        // Task 6 / Извлечение методов
        private void PrintOrderId(Order order)
            => Console.WriteLine("Order Id: " + order.Id);
        private void PrintTotal(Order order)
            => Console.WriteLine("Total: " + order.Total);
        private void PrintExpressShipping(Order order)
            => Console.WriteLine("Express Shipping: " + (order.IsExpress ? "Yes" : "No"));
        private void PrintCustomerEmail(Order order)
        {
            // Task 5 / Вызов объекта из класа Customer
            order.Customer.PrintEmail();
        }
    }

    public class OrderPriceCalculator
    {
        // Task 1 / Замена имени переменной
        const double taxRate = 0.2; // НДС
                                    // скидка 10% при заказе от 10000

        // Task 2 / Осмысленные имена для магических чисел
        const double discountThreshold = 10000; // Порог для получения скидки
        const double discountRate = 0.1;        // Размер скидки

        // итоговая цена
        public double Calculate(Order order)
        {
            double discount = CalculateDiscount(order.Total);
            double tax = CalculateTax(order.Total);

            return order.Total - discount + (order.Total * taxRate);
        }

        private double CalculateDiscount(double total)
        {
            // Task 3 / Тернарный оператор
            return total > discountThreshold ? total * discountRate : 0;
        }

        private double CalculateTax(double total)
        {
            return total * taxRate;
        }
    }
}
