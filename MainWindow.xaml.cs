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
using System.Collections.ObjectModel;

namespace Home_Work_10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool BOT_ON_OFF = false;
        public static bool TOKEN_SET_or_NOT = false;
        public static bool STORAGE_SET_or_NOT = false;


        public MainWindow()
        {
            InitializeComponent();

            TitleTextBoxL.Text = "Домашнее задание к лекции №10\nТема лекции: Основы WPF";
            TitleTextBoxR.Text = "Студент: Минкин Владимир\nЛектор: Камянецкий Сергей";

            BotControll.StartingChecks();
            if (TOKEN_SET_or_NOT)   TokenEnterBar.Text = "Loaded from file";
            if (STORAGE_SET_or_NOT) StoragePathEnterBar.Text = "Loaded from file";
            
            StatusRefresh();
        }

        /* Вкладка Main Controll */
        private void StatusRefresh()
        {
            if (BOT_ON_OFF)
                 Bot_Status.Text = "BOT status: active";
            else Bot_Status.Text = "BOT status: inactive";

            if (TOKEN_SET_or_NOT)
                 Token_Status.Text = "TOKEN status: is set";
            else Token_Status.Text = "TOKEN status: need input";

            if (STORAGE_SET_or_NOT)
                 Storage_Status.Text = "STORAGE status: is set";
            else Storage_Status.Text = "STORAGE status: path not selected";
        }

        private void ClientStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!BOT_ON_OFF && TOKEN_SET_or_NOT && STORAGE_SET_or_NOT)
            {
                BotControll.Bot_Initializing();
                BotLogList.ItemsSource = BotControll.BotLog;
                BOT_ON_OFF = true;
                StatusRefresh();
            }
            else if (!TOKEN_SET_or_NOT && !STORAGE_SET_or_NOT) { MessageBox.Show("Токен и хранилище не заданы.", this.Title, MessageBoxButton.OK); }
            else if (!TOKEN_SET_or_NOT)                        { MessageBox.Show("Токен не задан.",              this.Title, MessageBoxButton.OK); }
            else if (!STORAGE_SET_or_NOT)                      { MessageBox.Show("Хранилище не задано.",         this.Title, MessageBoxButton.OK); }
        }

        private void StopBotBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BOT_ON_OFF)
            {
                BotControll.Bot_Stop();
                BOT_ON_OFF = false;
                StatusRefresh();
            }
            else MessageBox.Show("Бот ещё не запущен.", this.Title, MessageBoxButton.OK);
        }

        private void SetTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TokenEnterBar.Text != "Enter Token Here" && TokenEnterBar.Text != "" && TokenEnterBar.Text != "Loaded from file")
            {
                TOKEN_SET_or_NOT = true;
                if (BotControll.Bot_Token_Set(TokenEnterBar.Text))
                { MessageBox.Show("Токен был заменён.", this.Title, MessageBoxButton.OK); }
                else
                { MessageBox.Show("Токен установлен.", this.Title, MessageBoxButton.OK); }
                return;
            }
            else
            { MessageBox.Show("Введите Токен.", this.Title, MessageBoxButton.OK); }
            StatusRefresh();
        }

        private void SetStorageBtn_Click(object sender, RoutedEventArgs e)
        {            
            if (StoragePathEnterBar.Text != "Enter Storage Path Here" && StoragePathEnterBar.Text != "" && StoragePathEnterBar.Text != "Loaded from file")
            {
                STORAGE_SET_or_NOT = true;
                if (BotControll.Bot_Storage_Set(StoragePathEnterBar.Text))
                { MessageBox.Show("Путь к хранилищу был заменён.", this.Title, MessageBoxButton.OK); }
                else
                { MessageBox.Show("Путь к хранилищу задан.", this.Title, MessageBoxButton.OK); }                
                return;
            }
            else
            { MessageBox.Show("Введите Путь к хранилищу.", this.Title, MessageBoxButton.OK); }
            StatusRefresh();
        }

        private void TokenEnterBar_GotMouseCapture(object sender, MouseEventArgs e)
        { if (TokenEnterBar.Text == "Enter Token Here" || TokenEnterBar.Text == "Loaded from file") TokenEnterBar.Text = ""; }

        private void StoragePathEnterBar_GotMouseCapture(object sender, MouseEventArgs e)
        { if(StoragePathEnterBar.Text == "Enter Storage Path Here" || StoragePathEnterBar.Text == "Loaded from file") StoragePathEnterBar.Text = ""; }

        /* Вкладка Update Log */

        public void BotLogInitialize()
        {
            BotLogList.ItemsSource = BotControll.BotLog;
        }

        private void LoadLogBtn_Click(object sender, RoutedEventArgs e)
        {
           // BotLogUpdate();
        }

        private void GridViewColumn_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            
        }
    }
}
