using System;
using System.Windows.Input;

namespace Encryptor.ViewModel
{
    public class RelayCommand<TParametre> : ICommand
    {
        private readonly Action<TParametre> _logiqueExecution;
        private readonly Func<TParametre, bool> _indicateurExecution;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<TParametre> logiqueExecution)
            : this(logiqueExecution, _ => true) { }

        public RelayCommand(Action<TParametre> logiqueExecution, Func<TParametre, bool> indicateurExecution)
        {
            if (logiqueExecution != null)
            {
                _logiqueExecution = logiqueExecution ;
            }
            else
            {
                throw new ArgumentNullException(nameof(logiqueExecution), "La logique d'exécution est obligatoire.");
            }
            // _logiqueExecution = logiqueExecution ?? throw new ArgumentNullException(nameof(logiqueExecution), "La logique d'exécution est obligatoire.");

            if (indicateurExecution != null)
            {
                _indicateurExecution = indicateurExecution;
            }
            else
            {
                throw new ArgumentNullException(nameof(logiqueExecution), "L'indicateur d'exécution est obligatoire.");
            }
            

            //_indicateurExecution = indicateurExecution ?? throw new ArgumentNullException(nameof(logiqueExecution), "L'indicateur d'exécution est obligatoire.");
        }

        public bool CanExecute(object parametre) => _indicateurExecution((TParametre)parametre);

        public void Execute(object parametre) => _logiqueExecution((TParametre)parametre);
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action logiqueExecution) : this(logiqueExecution, () => true) { }
        public RelayCommand(Action logiqueExecution, Func<bool> indicateurExecution)
            : base(_ => logiqueExecution(), _ => indicateurExecution()) { }
    }
}
