using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Categories // ◄ NAMESPACE CORRIGIDO
{
    public partial class CategoriesPage : ContentPage
    {
        public CategoriesPage(CategoriesViewModel viewModel)
        {
            InitializeComponent(); // ◄ Agora funciona!
            BindingContext = viewModel;
        }
    }
}