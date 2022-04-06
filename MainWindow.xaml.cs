using Language_2ISP11_17_DanArt_ArtZey.BD;
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

namespace Language_2ISP11_17_DanArt_ArtZey
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            listReader.ItemsSource = AppData.Context.Client.ToList();

            List<Gender> genders = AppData.Context.Gender.ToList();
            genders.Insert(0, new Gender() { NameGender = "Все" });
            cmbSortGender.ItemsSource = genders;
            cmbSortGender.DisplayMemberPath = "NameGender";
            cmbSortGender.SelectedIndex = 0;

            cmbSort.SelectedIndex = 0;
            cmbSort.ItemsSource = new List<String>
            {
                "По умолчанию",
                "По фамилии (В алфавитном порядке)",
                "По дате последнего посещения (От новых к старым)",
                "По количеству посещений (От большего к меньшему)"
            };

            cmbCountOnPage.SelectedIndex = 3;
            cmbCountOnPage.ItemsSource = new List<String>
            {
                "10",
                "50",
                "200",
                "Все"
            };



            Filter();
        }

        /// <summary>
        /// Метод для фильтрации и сортировки списка клиента
        /// </summary>
        private void Filter()
        {
            //Поиск по полю
            var list = AppData.Context.Client.ToList();
            list = list.Where(i => i.FirstName.ToLower().Contains(txtSearch.Text.ToLower())
                            || i.LastName.ToLower().Contains(txtSearch.Text.ToLower())
                            || i.Patronymic.ToLower().Contains(txtSearch.Text.ToLower())
                            || i.Mail.ToLower().Contains(txtSearch.Text.ToLower())
                            || i.PhoneNumber.ToLower().Contains(txtSearch.Text.ToLower())).ToList();

            //Фильтрация по полу
            if (cmbSortGender.SelectedIndex == 0)
            {    
            }
            else
            {
                var gender = cmbSortGender.SelectedItem as Gender;
                list = list.Where(i => i.IDGender == gender.ID).ToList();
            }

            //Сортировка
            switch(cmbSort.SelectedIndex)
            { 
                case 1:
                    list = list.OrderBy(i => i.LastName).ToList();
                    break;
                case 2:
                    list = list.OrderByDescending(i => i.LastVisit).ToList();
                    break;
                case 3:
                    list = list.OrderByDescending(i => i.CountVisits).ToList();
                    break;
                default:
                    list = list.OrderBy(i => i.ID).ToList();
                    break;
            }

            //Фильтрация по ДР (IsChecked)
            if (chBirthday.IsChecked == false)
            {
            }
            else
            { 
                list = list.Where(i => i.Birthday.Month == DateTime.Now.Month).ToList();    
            }

            //Фильтрация по кол-ву записей
            switch (cmbCountOnPage.SelectedIndex)
            {
                case 1:
                    list = list.OrderBy(i => 10).ToList();
                    break;
                case 2:
                    list = list.OrderBy(i => 50).ToList();
                    break;
                case 3:
                    list = list.OrderBy(i => 200).ToList();
                    break;
                default:
                    list = list.OrderBy(i => 999).ToList();
                    break;
            }
          listReader.ItemsSource = list;
        }
        private void CmbSortGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void ChBirthday_Checked(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void ChBirthday_Unchecked(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            listReader.ItemsSource = AppData.Context.Client.ToList();
            txtSearch.Text = "";
            cmbSort.SelectedIndex = 0;
            cmbSortGender.SelectedIndex = 0;
            cmbCountOnPage.SelectedIndex = 3;
        }

        private void CountOnPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void BtnUserDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listReader.SelectedItem is Client client)
            {
                var result = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление клиента", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (AppData.Context.ClientService.Where(i=> i.IDClient == client.ID).FirstOrDefault()!=null)
                    {
                       MessageBox.Show("У клиента были посещения ранее \nУдаление запрещено!", "Удаление клиента", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    else
                    {
                        AppData.Context.Client.Remove(client);
                        AppData.Context.SaveChanges();
                        MessageBox.Show("Запись успешно удалена", "Удаление клиента", MessageBoxButton.OK, MessageBoxImage.Information);
                        Filter();
                    }
                }
            }

            else
            {
                MessageBox.Show("Выберите клиента", "Удаление клиента", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
