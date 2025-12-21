using MagazAvtoZap.DataAccess;

namespace MagazAvtoZap.BusinessLogic
{
    public class InventoryService
    {
        private readonly DatabaseService _databaseService;

        public InventoryService()
        {
            _databaseService = new DatabaseService();
        }

        public bool CheckStock(int productId, int requestedQuantity)
        {
            int stock = _databaseService.GetProductStock(productId);
            return stock >= requestedQuantity;
        }
    }
}
