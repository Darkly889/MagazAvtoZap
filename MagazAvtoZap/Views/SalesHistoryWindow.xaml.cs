using System;
using System.Collections.Generic;
using System.Windows;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class SalesHistoryWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public SalesHistoryWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadSalesHistory();
        }

        private void LoadSalesHistory()
        {
            var salesHistory = _databaseService.GetSalesHistory();
            SalesHistoryDataGrid.ItemsSource = salesHistory;
        }
    }
}
