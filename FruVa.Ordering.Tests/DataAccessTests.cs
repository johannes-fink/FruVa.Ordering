using FruVa.Ordering.DataAccess.Models;

namespace FruVa.Ordering.Tests
{
    [TestClass]
    public class DataAccessTests
    {
        private static DataAccess.Context? _db;

        [TestInitialize]
        public void Setup()
        {
            _db = new DataAccess.Context();
        }

        [TestMethod]
        public void CanSaveOrder()
        {
            var order = new Order
            {
                OrderNumber = 20,
                RecipientId = Guid.NewGuid(),
                OrderDetails =
                [
                    new()
                    {
                        ArticleId = Guid.NewGuid(),
                        Price = 100,
                        Quantity = 1,
                    }
                ]
            };

            try
            {
                _db!.Orders.Add(order);
                _db.SaveChanges();
            }
            finally
            {
                _db!.Orders.Remove(order);
                _db.SaveChanges();
            }
        }
    }
}
