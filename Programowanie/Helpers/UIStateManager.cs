using FitnessApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FitnessApp.Helpers
{
    public class UIStateManager : BaseViewModel
    {
        private Visibility _scanVisibility;
        private Visibility _inputVisibility;
        private Visibility _backButtonVisibility;
        private Visibility _startPanelVisibility;

        public Visibility ScanVisibility
        {
            get => _scanVisibility;
            set => SetProperty(ref _scanVisibility, value);
        }

        public Visibility InputVisibility
        {
            get => _inputVisibility;
            set => SetProperty(ref _inputVisibility, value);
        }

        public Visibility BackButtonVisibility
        {
            get => _backButtonVisibility;
            set => SetProperty(ref _backButtonVisibility, value);
        }

        public Visibility StartPanelVisibility
        {
            get => _startPanelVisibility;
            set => SetProperty(ref _startPanelVisibility, value);
        }

        // Inicjalizacja początkowych stanów
        public UIStateManager()
        {
            ScanVisibility = Visibility.Collapsed; // Na początku kamera ukryta
            InputVisibility = Visibility.Collapsed; // Na początku pole do wpisania nazwy produktu ukryte
            BackButtonVisibility = Visibility.Collapsed; // Na początku brak przycisku do powrotu
            StartPanelVisibility = Visibility.Visible; // Na początku widoczne przyciski
        }

        // Zmiana na tryb skanowania
        public void SetScanMode()
        {
            ScanVisibility = Visibility.Visible;
            InputVisibility = Visibility.Collapsed;
            BackButtonVisibility = Visibility.Visible;
            StartPanelVisibility = Visibility.Collapsed; // Ukryj przyciski, kiedy jesteśmy w trybie skanowania
        }

        // Zmiana na tryb wpisywania nazwy produktu
        public void SetInputMode()
        {
            ScanVisibility = Visibility.Collapsed;
            InputVisibility = Visibility.Visible;
            BackButtonVisibility = Visibility.Visible;
            StartPanelVisibility = Visibility.Collapsed; // Ukryj przyciski, kiedy jesteśmy w trybie wpisywania nazwy
        }

        // Zmiana na tryb startowy
        public void SetStartMode()
        {
            ScanVisibility = Visibility.Collapsed;
            InputVisibility = Visibility.Collapsed;
            BackButtonVisibility = Visibility.Collapsed;
            StartPanelVisibility = Visibility.Visible; // Przywróć widoczność przycisków na głównym ekranie
        }
    }
}