using pr1_vnedrenit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Xaml;

namespace pr1_vnedrenit.Pages
{
    /// <summary>
    /// Логика взаимодействия для PartnersApplications.xaml
    /// </summary>
    public partial class PartnersApplications : Page
    {
        bool admin = false;
        private Zayavka _selectedZayavka;

        private List<Zayavka> allServices;

        //private bool isAdmin = false; // Поле для хранения статуса администратора

        public PartnersApplications()
        {
            InitializeComponent();

            LoadServices();

     
        }



        private List<Zayavka> GetZayavkaFromDatabase()
        {
            try
            {
                return pr1_vnedrenieEntities.GetContext().Zayavka.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запросе к базе данных: {ex.Message}");
                return new List<Zayavka>();
            }
        }
        // Обновленный метод с правильным Include
        private void LoadServices()
        {
            try
            {
                allServices = pr1_vnedrenieEntities.GetContext().Zayavka
                    //.Include(z => z.Partneri)
                    //.Include(z => z.Partneri.TipPartnera) // Подгружаем тип партнера
                    .ToList();

                ZayavkiList.ItemsSource = allServices;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddAndEdit(null));

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedZayavka != null)
            {
                // Переходим на страницу добавления/редактирования, передавая выбранную заявку для редактирования
                NavigationService?.Navigate(new AddAndEdit(_selectedZayavka));
            }
            else
            {
                MessageBox.Show("Выберите заявку для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void ZayavkiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedZayavka = ZayavkiList.SelectedItem as Zayavka;
            // Кнопка "Редактировать" станет активной только когда что-то выбрано
            btnUpdate.IsEnabled = (_selectedZayavka != null);
        }

        private void isv_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadServices();

        }
    }
}
