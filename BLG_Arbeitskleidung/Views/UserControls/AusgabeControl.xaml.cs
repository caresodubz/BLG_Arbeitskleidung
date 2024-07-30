using System.Windows.Controls;

namespace BLG_Arbeitskleidung.Views.UserControls {
    public partial class AusgabeControl : UserControl {
        public AusgabeControl() {
            InitializeComponent();
        }

        private void OnMengeTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            if(!int.TryParse(e.Text, out _) || string.IsNullOrWhiteSpace(e.Text)) {
                e.Handled = true;
            }
        }
    }
}
