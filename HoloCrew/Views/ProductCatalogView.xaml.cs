using System.Windows.Controls;

namespace HoloCrew.Views
{
    /// <summary>
    /// Product catalog view with horizontal filters (no sidebar)
    /// No scroll handlers needed - MainWindow handles scroll
    /// </summary>
    public partial class ProductCatalogView : UserControl
    {
        public ProductCatalogView()
        {
            InitializeComponent();
        }
    }
}