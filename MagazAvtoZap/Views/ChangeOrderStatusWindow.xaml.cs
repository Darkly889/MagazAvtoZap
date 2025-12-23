using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.DataAccess;

namespace MagazAvtoZap.Views
{
    public partial class ChangeOrderStatusWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private string _orderNumber;

        public ChangeOrderStatusWindow(string orderNumber)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _orderNumber = orderNumber;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Выберите статус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _databaseService.UpdateOrderStatus(_orderNumber, newStatus);
            this.DialogResult = true;
            this.Close();
        }
    }
}
