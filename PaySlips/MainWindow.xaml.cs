using Payslips.Data;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;
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
                string path = string.Empty;
                List<Person> persons = new();
                path = openFileDialog.FileName;
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string? line = string.Empty;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (emailChecker.IsMatch(line))
                        {
                            persons.Add(new Person(line.Split('\t')[0], line.Split("\t")[1]));
                        }
                    }
                }
                foreach (var person in persons)
                {
                    Database.Add(person);
                }
                PersonalList.ItemsSource = Database.GetPersonal();
            }
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EmailTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.ToString()) || string.IsNullOrEmpty(SMTPTextBox.Text) || string.IsNullOrEmpty(SMTPPortTextBox.Text)||string.IsNullOrEmpty(PdfPath.Text))
            {
                MessageBox.Show("Проверьте заполненность полей \"Email\", \"Пароль\", \"SMTP\", \"SMTP порт\", \"Путь к файлу\"");
                return;
            }
            int port;
            List<Person> Personal = new List<Person>();
            if (int.TryParse(SMTPPortTextBox.Text, out port)) { }
            else
            {
                MessageBox.Show("Ошибка в поле SMTP порт!");
                return;
            }
            PdfEditor pdfEditor = new PdfEditor(PdfPath.Text);
            PdfEditor.PathSave = SavePdfPathTextBox.Text;
            foreach (Person person in PersonalList.SelectedItems)
            {
                Personal.Add(person);
            }
            pdfEditor.Divide();
            MessageSender messageSender = new MessageSender { SmtpPort = port, Smtp = SMTPTextBox.Text, Message = MessageTextBox.Text };
            messageSender.Send(EmailTextBox.Text, PasswordTextBox.Password, Personal);
            pdfEditor.DeleteDirectory(!SaveFilesCheckBox.IsChecked);
            MessageBox.Show("Отправка завершена!");
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

            if (folderBrowser.ShowDialog()==WinForms.DialogResult.OK)
            {
                SavePdfPathTextBox.Text = folderBrowser.SelectedPath;
            }
        }
    }
}
