using System.Windows.Controls;
using System.Windows.Input;

namespace BLG_Arbeitskleidung.Views.UserControls {
    public partial class EinlagernControl : UserControl {
        public EinlagernControl() {
            InitializeComponent();
        }

        private void OnMengeTextInput(object sender, TextCompositionEventArgs e) {
            if(!int.TryParse(e.Text, out _) || string.IsNullOrWhiteSpace(e.Text)) {
                e.Handled = true;
            }
        }

        private void TextBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {

        }
    }
}