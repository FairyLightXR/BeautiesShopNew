using BeautiesShop.Entity_Model;
using BeautiesShop.Other;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        private Product addProduct;
        public string Path { get { return System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "../../UserInterface")); } }
        public AddEditProduct(Product tempProd)
        {
            InitializeComponent();
            addProduct = tempProd ?? new Product();
            HeaderBlock.Text = tempProd != null ? "Редактирование продукта" : "Добавление продукта";

            if (tempProd != null)
                ImageProduct.Source = (ImageSource)new ImageSourceConverter().ConvertFromString($@"{Path}\{tempProd.MainImagePath}");

            var allManufacturer = Transition.Datacontext.Manufacturer.ToList();
            ManufacturerCBox.ItemsSource = allManufacturer;

            //DataContext = addProduct;

            //В разработке
            //AttachedProdLV.ItemsSource = Transition.Datacontext.AttachedProduct.ToList();
            //AttachedProdLV.DataContext = Transition.Datacontext.AttachedProduct.ToList();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (string.IsNullOrWhiteSpace(addProduct.Title))
                error.AppendLine("Укажите наименование");
            if (string.IsNullOrWhiteSpace(addProduct.Cost.ToString()))
                error.AppendLine("Укажите цену");
            if (addProduct.Cost.ToString().StartsWith("-"))
                error.AppendLine("Цена не может быть отрицательной");
            if (addProduct.Manufacturer == null)
                error.AppendLine("Выберите производителя");
            if (string.IsNullOrWhiteSpace(addProduct.MainImagePath))
                error.AppendLine("Загрузите фото");
            if (string.IsNullOrWhiteSpace(addProduct.Description))
                addProduct.Description = "";

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (addProduct.ID == 0)
                Transition.Datacontext.Product.Add(addProduct);

            try
            {
                Transition.Datacontext.SaveChanges();
                MessageBox.Show($"Данные успешно сохранены", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Information);
                Transition.MainFrame.GoBack();
            }
            catch (Exception er)
            {
                MessageBox.Show($"При сохранении продукта произошла ошибка:\n{er.Message}", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog downloadImage = new OpenFileDialog();
            downloadImage.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            downloadImage.InitialDirectory = $@"{Path}\Товары салона красоты";

            if ((bool)downloadImage.ShowDialog())
            {
                if (!File.Exists(Path + "\\Товары салона красоты\\" + downloadImage.SafeFileName))
                    File.Copy(downloadImage.FileName, Path + "\\Товары салона красоты\\" + downloadImage.SafeFileName);

                addProduct.MainImagePath = $@"Товары салона красоты\{downloadImage.SafeFileName}";
                ImageProduct.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(downloadImage.FileName);
            }
        }
    }
}
