using Encryptor.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Encryptor.ViewModel
{
    public class DetailFichier : INotifyPropertyChanged
    {
        private string _name;
        private bool _isChecked = true;

        public string Name
        {
            get { _name; } 
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                //EncryptorViewModel.CheckedClicked(this);
                OnPropertyChanged();
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public string Fichier { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
