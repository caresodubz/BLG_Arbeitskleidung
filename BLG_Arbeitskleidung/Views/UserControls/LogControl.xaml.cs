using BLG_Arbeitskleidung.Models;
using System.Windows;
using System.Windows.Controls;

namespace BLG_Arbeitskleidung.Views.UserControls {
    public partial class LogControl : UserControl {
        public LogControl() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            ExcelListeErzeugen excelListeErzeugen = new();
            excelListeErzeugen.ErstelleExcelListe();
        }
    }
}
