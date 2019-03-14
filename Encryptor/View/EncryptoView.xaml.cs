using System.Windows;
using Encryptor.ViewModel;

namespace Encryptor.View
{
    /// <summary>
    /// Interaction logic for EncryptoView.xaml
    /// </summary>
    public partial class EncryptoView : Window
    {
        public EncryptoView()
        {
            InitializeComponent();
            DataContext = EncryptorViewModel;
        }
    }
}
