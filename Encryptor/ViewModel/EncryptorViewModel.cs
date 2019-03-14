using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Encryptor.ViewModel
{
    public class EncryptorViewModel : ViewModelBase
    {
        private const string JSONFILTER = "Json files(*.json)|*.json";

        private string _inputPath;
        private string _predictiveWarning;

        private static readonly ObservableCollection<DetailFichier> _listeFichier = new ObservableCollection<DetailFichier>();

        public ICommand OnEncrypt { get; }
        public ICommand OnDecrypt { get; }
        public ICommand OnClearCache { get; }
        public ICommand OnParcourir { get; }
        public ICommand OnChargerListeFichier { get; }
        public ICommand OnSupprimerFichier { get; }

        public static ObservableCollection<DetailFichier> ListeFichier => _listeFichier;

        public EncryptorViewModel()
        {
            OnEncrypt = new RelayCommand(OnEncryptClicked);
            OnDecrypt = new RelayCommand(OnDecryptClicked);
            OnClearCache = new RelayCommand(OnClearCacheClicked);
            OnParcourir = new RelayCommand(OnParcourirClicked);
            OnChargerListeFichier = new RelayCommand(OnChargerListeFichierClicked);
            OnSupprimerFichier = new RelayCommand<DetailFichier>(OnSupprimerFichierClicked);
        }

        public string InputPath
        {
            get { _inputPath; } 
            set
            {
                if (_inputPath != value)
                {
                    _inputPath = value;
                    OnPropertyChanged();
                    UpdatePredictiveWarning();
                }
            }
        }

        public string PredictiveWarning
        {
            get { _predictiveWarning; } 
            private set
            {
                _predictiveWarning = value;
                OnPropertyChanged();
            }
        }

        public void OnEncryptClicked()
        {
            var path = Path.GetDirectoryName(InputPath);
            var exceptions = new List<string>();

            foreach (var file in ListeFichier)
            {
                var fileName = $"{Path.GetFileNameWithoutExtension(file.Name)}-Encrypté";
                try
                {
                    var result = Model.EncryptorModel.EncryptFile(file.Name);
                    var newPath = $"{path}\\{fileName}.json";
                    File.WriteAllBytes(newPath, result);
                }
                catch (Exception)
                {
                    exceptions.Add(fileName);
                }
            }
            var count = ListeFichier.Count - exceptions.Count;
            if (exceptions.Any())
            {
                MessageBox.Show(
                    $"L’encryption des fichiers suivants a échoué: {exceptions.Aggregate((str, val) => str + Environment.NewLine + val)}",
                    "Erreur d'encryption", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Encryption réussie: {count} fichier(s)");
            }
        }

        public void OnDecryptClicked()
        {
            var path = Path.GetDirectoryName(InputPath);
            var exceptions = new List<string>();

            foreach (var file in ListeFichier)
            {
                var fileName = $"{Path.GetFileNameWithoutExtension(file.Name)}-Decrypté";
                try
                {
                    var result = Model.EncryptorModel.DecryptFile(file.Name);
                    var newPath = $"{path}\\{fileName}.json";
                    File.WriteAllText(newPath, result);
                }
                catch (Exception)
                {
                    exceptions.Add(fileName);
                }

                var count = ListeFichier.Count - exceptions.Count;
                if (exceptions.Any())
                    MessageBox.Show(
                        $"La décryption des fichiers suivants a échoué: {exceptions.Aggregate((str, val) => str + Environment.NewLine + val)}",
                        "Erreur de décryption", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    MessageBox.Show($"Décryption réussie pour: {count} fichier(s)");
                }
            }
        }

        public void OnClearCacheClicked()
        {
            try
            {
                ListeFichier.Clear();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Erreur de suppression fichiers", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnParcourirClicked()
        {
            try
            {
                var openFileDialog = new OpenFileDialog { Filter = JSONFILTER, Multiselect = true };
                var showDialog = openFileDialog.ShowDialog();
                var detail = new DetailFichier { Name = openFileDialog.FileName };
                var isChecked = detail.IsChecked;
                if (showDialog == true && isChecked)
                {
                    InputPath = openFileDialog.FileName;
                    ListeFichier.Add(detail);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ouvrir", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSupprimerFichierClicked(DetailFichier detailFichier)
        {
            ListeFichier.Remove(detailFichier);
        }

        public static void CheckedClicked(DetailFichier detail)
        {
            if (detail.IsChecked)
            {
                ListeFichier.Add(detail);
            }
            else
            {
                ListeFichier.Remove(detail);
            }
        }

        public static void OnChargerListeFichierClicked()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = JSONFILTER,
                Multiselect = true
            };
            var showDialog = openFileDialog.ShowDialog();
            if (showDialog != true)
                return;

            ListeFichier.Clear();
            foreach (var fileName in openFileDialog.FileNames)
            {
                var fichier = new DetailFichier { Fichier = fileName };
                ListeFichier.Add(fichier);
            }
        }


        private void UpdatePredictiveWarning()
        {
            if (string.IsNullOrEmpty(InputPath))
            {
                PredictiveWarning = string.Empty;
            }
            else
            {
                PredictiveWarning = Model.EncryptorModel.CanFileBeDecrypted(InputPath) ? "File is encrypted." : "File can't be decrypted.";
            }
        }
    }
}
