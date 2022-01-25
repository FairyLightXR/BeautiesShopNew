using BeautiesShop.Entity_Model;
using BeautiesShop.Other;
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

namespace BeautiesShop.UserInterface.Pages
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : Page
    {
        private List<Product> ProdList { get { return Transition.Datacontext.Product.ToList(); } }
        public ProductView()
        {
            InitializeComponent();
            //Заполнение данными combobox, добавление заглушки "Все производители" для нулевого индексв
            var cmbManufacturer = Transition.Datacontext.Manufacturer.ToList();
            cmbManufacturer.Insert(0, new Manufacturer { Name = "Все производители" });
            ManufacturerCBox.ItemsSource = cmbManufacturer;
            ManufacturerCBox.SelectedIndex = 0;
            SortCBox.SelectedIndex = 0;

            ViewProduct.ItemsSource = Transition.Datacontext.Product.ToList();

        }

        #region Обновление данных ListView после изменений
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Transition.Datacontext.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                SortingProduct();
            }
        }
        #endregion

        #region Реализация динамического поиска и сортировки данных
        private void SortingProduct()
        {
            var itemUpdate = Transition.Datacontext.Product.ToList();

            if (ManufacturerCBox.SelectedIndex > 0)
                itemUpdate = itemUpdate
                    .Where(p => p.ManufacturerID == (ManufacturerCBox.SelectedItem as Manufacturer).ID)
                    .ToList();

            if (SearchBox.Text != "Введите для поиска")
                itemUpdate = itemUpdate
                    .Where(p => p.Title.ToLower().Contains(SearchBox.Text.ToLower())
                    || p.Description.ToLower().Contains(SearchBox.Text.ToLower()))
                    .ToList();

            switch (SortCBox.SelectedIndex)
            {
                case 1:
                    {
                        if (!(bool)OrderCheck.IsChecked)
                            itemUpdate = itemUpdate.OrderBy(p => p.Cost).ToList();
                        else
                            itemUpdate = itemUpdate.OrderByDescending(p => p.Cost).ToList();
                        break;
                    }
            }

            CountProduct.Text = $"Количество записей: {itemUpdate.Count} из {ProdList.Count}";

            if (itemUpdate.Count > 0)
            {
                ViewProduct.Visibility = Visibility.Visible;
                ViewProduct.ItemsSource = itemUpdate;
            }
            else
                ViewProduct.Visibility = Visibility.Hidden;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text != "Введите для поиска" && SearchBox.Text != null)
                SortingProduct();
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text != null)
                SearchBox.Text = "Введите для поиска";
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = null;
        }

        private void SortCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
        }

        private void OrderCheck_Checked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
        }

        private void OrderCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
        }

        private void ManufacturerCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
        } 
        #endregion

        private void ViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tempCountItems = ViewProduct.SelectedItems.Count;

            if (tempCountItems != 0)
            {
                if (tempCountItems == 1)
                    Transition.MainFrame.Navigate(new AddEditProduct(ViewProduct.SelectedItem as Product));
                else
                    MessageBox.Show("Необходимо выбрать конкретный продукт", "Редактирование продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Выберите продукт для редактирования", "Редактирование продукта", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Transition.MainFrame.Navigate(new AddEditProduct(null));
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var deleteDataProd = ViewProduct.SelectedItems.Cast<Product>().ToList();

            foreach (var itemProdSale in Transition.Datacontext.ProductSale.ToList())
            {
                foreach (var itemProd in deleteDataProd)
                {
                    if (itemProd.ID == itemProdSale.ProductID)
                    {
                        MessageBox.Show("Один из выбранных продуктов имеет информацию о его продажах", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            if (MessageBox.Show($"Вы хотите удалить {deleteDataProd.Count} элементов?",
                "Удаление данных", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Transition.Datacontext.Product.RemoveRange(deleteDataProd);
                    Transition.Datacontext.SaveChanges();
                    MessageBox.Show("Данные успешно удалены", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Information);
                    SortingProduct();
                }
                catch (Exception er)
                {
                    MessageBox.Show($"При удалении данных произошла ошибка:\n{er.Message}", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        /// <summary>
        /// Появление и сокрытие кнопки удалить, если выбран продукт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewProduct.SelectedItems.Count > 0)
                DeleteBtn.Visibility = Visibility.Visible;
            else
                DeleteBtn.Visibility = Visibility.Hidden;
        }
    }
}
