using pr1_vnedrenit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pr1_vnedrenit.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddAndEdit.xaml
    /// </summary>
    public partial class AddAndEdit : Page, INotifyPropertyChanged
    {
        private pr1_vnedrenieEntities _context = pr1_vnedrenieEntities.GetContext();
        private Zayavka _currentZayavka;
        private Partneri _currentPartner;

        // Режим работы: true - редактирование, false - добавление
        private bool _isEditing;

        public string Title => _isEditing ? "Редактирование заявки партнера" : "Добавление новой заявки партнера";

        public Partneri CurrentPartner
        {
            get => _currentPartner;
            set
            {
                _currentPartner = value;
                OnPropertyChanged();
            }
        }

        public List<Id_tip_partnera> TipyPartnerov { get; set; }

        public AddAndEdit(Zayavka selectedZayavka)
        {
            InitializeComponent();
            DataContext = this; // Устанавливаем контекст данных для привязки

            // Загружаем список типов партнеров для ComboBox
            TipyPartnerov = _context.Id_tip_partnera.ToList();

            _isEditing = (selectedZayavka != null);
            _currentZayavka = selectedZayavka ?? new Zayavka();

            cmbTipPartnera.ItemsSource = TipyPartnerov;

            // Если редактируем, берем существующего партнера, иначе создаем нового
            if (_isEditing)
            {
                CurrentPartner = _currentZayavka.Partneri;

            }
            else
            {
                CurrentPartner = new Partneri();
                // Можно установить значения по умолчанию, например:
                CurrentPartner.reyting = "0";
            }
        }

        // Реализация INotifyPropertyChanged для обновления привязок
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        // Валидация ввода для рейтинга (только цифры)
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool ValidateData()
        {
            StringBuilder errors = new StringBuilder();

            if (cmbTipPartnera.SelectedItem == null)
                errors.AppendLine("Не выбран тип партнера.");
            if (string.IsNullOrWhiteSpace(CurrentPartner.naimenovanie_company))
                errors.AppendLine("Не заполнено наименование компании.");
            if (string.IsNullOrWhiteSpace(CurrentPartner.reyting))
                errors.AppendLine("Не заполнен рейтинг.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            try
            {
                // Если добавляем новую заявку
                if (!_isEditing)
                {
                    // Сначала сохраняем нового партнера
                    _context.Partneri.Add(CurrentPartner);
                    _context.SaveChanges(); // Сохраняем, чтобы получить ID партнера

                    // Связываем заявку с партнером
                    _currentZayavka.Id_partner = CurrentPartner.Id_partner;
                    _currentZayavka.data_sozdanie = DateTime.Now;
                    // ... установите другие поля заявки по умолчанию ...

                    _context.Zayavka.Add(_currentZayavka);
                }
                // Если редактируем, изменения в CurrentPartner уже отслеживаются контекстом

                _context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Возвращаемся назад
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Отменяем изменения, если они не сохранены (отслеживаются контекстом)
            // Для простоты просто вернемся назад
            var result = MessageBox.Show("Все несохраненные изменения будут потеряны. Продолжить?",
                                          "Подтверждение",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService?.GoBack();
            }
        }
    }
}
