using Payslips.Data;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace Payslips
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PersonalList.ItemsSource = Database.GetPersonal();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxWithPersonName.Text) || string.IsNullOrEmpty(TextBoxWithPersonEmail.Text))
            {
                MessageBox.Show("Заполните поля ФИО и Email");
            }
            else
            {
                var person = new Person(TextBoxWithPersonName.Text, TextBoxWithPersonEmail.Text);
                Database.Add(person);
                PersonalList.ItemsSource = Database.GetPersonal();
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalList.SelectedItems is var persons)
            {
                foreach (Person person in persons)
                {
                    Database.Remove(person);
                }
                PersonalList.ItemsSource = Database.GetPersonal();
            }
        }

        private void FileDialog_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new WinForms.OpenFileDialog();
            openFileDialog.Filter = "pdf файлы (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                PdfPath.Text = openFileDialog.FileName;
            }
        }

        private void SelectAll_CheckBox(object sender, RoutedEventArgs e)
        {
            if (SelectAllRButton.IsChecked == true)
            {
                PersonalList.SelectAll();
            }
            else
            {
                PersonalList.UnselectAll();
            }
        }

        private void OutputPersonalListFromFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new WinForms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                Regex emailChecker = new Regex(@"[^\s]*@[a-z0-9.-]*");
                openFileDialog.Filter = "txt файлы (*.txt)|*.txt";

                using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
                {
                    string? line = string.Empty;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (emailChecker.IsMatch(line))
                        {
                            Database.Add(new Person(line.Split('\t')[0], line.Split("\t")[1]));
                        }
                    }
                }
                PersonalList.ItemsSource = Database.GetPersonal();
            }
        }
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            MessageSender messageSender = new MessageSender(SMTPTextBox.Text, SMTPPortTextBox.Text, MessageTextBox.Text, EmailTextBox.Text, PasswordTextBox.Password);
            if (messageSender.IsNotCorrect)
            {
                MessageBox.Show("Проверьте заполненность полей");
                return;
            }
            PdfEditor.PathSave = SavePdfPathTextBox.Text;
            messageSender.Send(PersonalList.SelectedItems.OfType<Person>().ToList(), new PdfEditor(PdfPath.Text), !SaveFilesCheckBox.IsChecked);//todo
        }

        private void SaveFiles_Checked(object sender, RoutedEventArgs e)
        {
            SaveFileButton.Visibility = Visibility.Visible;
            SavePdfPathTextBox.Visibility = Visibility.Visible;
        }

        private void SaveFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveFileButton.Visibility = Visibility.Hidden;
            SavePdfPathTextBox.Text = string.Empty;
            SavePdfPathTextBox.Visibility = Visibility.Hidden;
        }

        private void SaveFileDialog_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new WinForms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == WinForms.DialogResult.OK)
            {
                SavePdfPathTextBox.Text = folderBrowser.SelectedPath;
            }
        }
    }
}
