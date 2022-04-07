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
        //переменные для постраничной навигации 

        int numberPage = 0; //номер текущей страницы
        int countItem = 0; //кол-во записей на стрн
        int countPage = 0; //кол-во страниц

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

            cmbCountOnPage.SelectedIndex = 0;
            cmbCountOnPage.ItemsSource = new List<String>
            {
                "Все",
                "10",
                "50",
                "200"
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

            //Постраничный вывод
            //Получаем количество странич
            switch (cmbCountOnPage.SelectedIndex)
            {
                case 0:
                    countItem = list.Count;
                    break;
                case 1:
                    countItem = 10;
                    break;
                case 2:
                    countItem = 50;
                    break;
                case 3:
                    countItem = 200;
                    break;
                default:
                    countItem = list.Count;
                    break;
            }
            if (list.Count/ countItem == 0|| list.Count / countItem == 1)
            {
                countPage = (list.Count / countItem);
            }
            else
            {
                countPage = (list.Count / countItem) + 1;
            }

            TBAllCountList.Text = list.Count.ToString();

            list = list.Skip(numberPage * countItem).Take(countItem).ToList();

            TBCountList.Text = list.Count.ToString();

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
            cmbCountOnPage.SelectedIndex = 0;
        }

        private void CountOnPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            numberPage = 0;
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
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (numberPage + 1 < countPage)
            {
                numberPage++;
            }
          Filter();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (numberPage > 0)
            {
                numberPage--;
            }
          Filter();
        }
    }
}
