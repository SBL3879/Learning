using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Home_Work_10
{
    public class BotControll
    {
        public static string token { get; set; } // загрузка токена из файла

        static ITelegramBotClient client; // инициализация бота на основе токена

        public static string storagePath; // путь к месту хранения файлов

        static Repository Storage; // лист хранимых файлов

        public static ObservableCollection<IncomingUpdate> BotLog { get; set; }

        public static CancellationTokenSource cts = new CancellationTokenSource();

        public static void StartingChecks()
        {
            if (System.IO.File.Exists("_token.txt"))
            {
                token = System.IO.File.ReadAllText("_token.txt");
                MainWindow.TOKEN_SET_or_NOT = true;
            }
            if (System.IO.File.Exists("_storagePath.txt"))
            {
                storagePath = System.IO.File.ReadAllText("_storagePath.txt");
                MainWindow.STORAGE_SET_or_NOT = true;
            }
        }

        public static bool Bot_Token_Set(string token)
        {
            bool Replace = false;

            if (System.IO.File.Exists("_token.txt"))
            {
                if (System.IO.File.ReadAllText("_token.txt") != token)
                {
                    Replace = true;
                }
            }

            System.IO.File.WriteAllText("_token.txt", token);
            
            return Replace;
        }

        public static bool Bot_Storage_Set(string path)
        {
            bool Replace = false;

            if (System.IO.File.Exists("_storagePath.txt"))
            {
                if (System.IO.File.ReadAllText("_storagePath.txt") != path)
                {
                    Replace = true;
                }
            }

            System.IO.File.WriteAllText("_storagePath.txt", path);

            return Replace;
        }

        public static void Bot_Initializing()
        {
            if(Storage is null)
            Storage = new Repository();
            if(BotLog is null)
            BotLog = new ObservableCollection<IncomingUpdate>();
            client = new TelegramBotClient(token);

            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

            client.StartReceiving(  // старт цикла оборота обновление/ответ
                HandleUpdateAsync,  // обработчик входящих сообщений
                HandleErrorAsync,   // обработчик входящих ошибок
                receiverOptions,    // ограничитель типа получаемых сообщений
                cancellationToken   // токен прерывания цикла StartReceiving
                );
        }

        public static void Bot_Stop() { cts.Cancel(); } // остановить StartReceiving

        public static void Bot_Log_Processor(string Operation, Update data)
        {
            switch (Operation)
            {
                case "Add New Record":
                    BotLog.Add( new IncomingUpdate()
                    {
                        Id          = data.Message.From.Id,
                        SendDate    = data.Message.Date.ToString(),
                        ReceiveDate = DateTime.Now.ToString(),
                        MsgType     = data.Message.Type.ToString(),
                        MsgText     = data.Message.Text,
                        FirstName   = data.Message.From.FirstName,
                        NickName    = data.Message.From.Username
                    });
                    break;
            }
        }

        static string responseText; // переменная хранящая ответное сообщение пользователю

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine(update.Type);
            if (update.Type.ToString() == "EditedMessage") return;
            Console.WriteLine(update.Message.Type);
            Console.WriteLine(@update.Message.Text);
            Console.WriteLine();


            if (update.Message.Type.ToString() == "Text")
            {
                string[] line = update.Message.Text.ToString().Split('_');

                switch (line[0])
                {
                    case "/start":/*----------------------------------------------------------------------------------------*/ //ответ на сообщение /start
                        responseText =                                                                                      //
                            $"Здравствуйте, {update.Message.From.FirstName}.\n" +                                           //
                            $"Данный бот является домашним заданием 9-го модуля, обучающей программы сайта SkillBox.\n" +   //
                            $"Студент: Минкин Владимир\n" +                                                                 //
                            $"Лектор: Камянецкий Сергей\n" +                                                                //
                            $"Подробности о командах вам расскажет команда /help.";                                         //
                        break;                                                                                              //
                    case "/help":/*-----------------------------------------------------------------------------------------*/ //ответ на сообщение /help
                        responseText =                                                                                      //
                            $"/start - приветствие и знакомство.\n" +                                                       //
                            $"/help - общее описание комманд.\n" +                                                          //
                            $"/filelist - просмотреть список имеющихся файлов для скачивания.\n" +                          //
                            $"/download_N - скачать файл с 'облака'. N - номер файла из списка.\n\n" +                      //
                            $"Чтобы поместить файл на хранение, просто отправьте его боту в лс.\n\n" +                      //
                            $"Примечание: " +                                                                               //
                            $" принимает фалы размером до 20 МБ.";                                                          //
                        break;                                                                                              //
                    case "/filelist":/*-------------------------------------------------------------------------------------*/ //ответ на сообщение /filelist
                        if (Storage.files_storage.Count == 0)                                                               //
                        {                                                                                                   //
                            responseText = "Хранилище файлов пустое. Пришлите боту файл для помещения его на хранение.";    //
                            break;                                                                                          //
                        }                                                                                                   //
                        int i = 1;                                                                                          //
                        foreach (StorageFile filename in Storage.files_storage)                                             //
                        {                                                                                                   //
                            responseText += $"№{i} - {filename.FileNAME}\n";                                                //
                            i++;                                                                                            //
                        }                                                                                                   //
                        break;                                                                                              //
                    case "/download":/*-------------------------------------------------------------------------------------*/ //ответ на сообщение /download_N
                        int ListNumber;                                                                                     //
                        try { ListNumber = Convert.ToInt32(line[1]); }                                                      //
                        catch (FormatException)                                                                             //
                        { responseText = "Напишите /help для получения описания возможных комманд."; break; }               //
                        var sendingFileID = Storage.files_storage[ListNumber - 1].FileID;                                   //
                        await client.SendDocumentAsync(update.Message.Chat, sendingFileID);                                 //
                        responseText = "Получите и распишитесь.";                                                           //
                        break;                                                                                              //
                    default:/*----------------------------------------------------------------------------------------------*/ //если сообщение не является допустимой командой                                                                                               //
                        responseText = "Напишите /help для получения описания возможных комманд.";                          //
                        break;                                                                                              //
                }/*---------------------------------------------------------------------------------------------------------*/
            }
            else if (update.Message.Type.ToString() == "Document")/*--------------------------------------------------------*/ //если тип полученного сообщения Document
            {                                                                                                               //
                var fileID = update.Message.Document.FileId;                                                                //
                var fileNAME = update.Message.Document.FileName;                                                            //
                var fromChat = update.Message.Chat.Id;                                                                      //
                var fromName = update.Message.From.Username;                                                                //
                                                                                                                            //
                if (Storage.CoincidenceCheck(fileNAME) == false)                                                            //
                {                                                                                                           //
                    Storage.AddNewFile(fileID, fileNAME, fromChat, fromName);                                               //
                                                                                                                            //
                    var file = await client.GetFileAsync(fileID);                                                           //
                                                                                                                            //
                    Console.WriteLine("FileNAME: " + fileNAME);                                                             //
                    Console.WriteLine("FilePath: " + file.FilePath + "\n");                                                 //
                                                                                                                            //
                    FileStream upload = new FileStream(storagePath + fileNAME, FileMode.Create);                            //
                    await client.DownloadFileAsync(file.FilePath, upload);                                                  //
                    upload.Close();                                                                                         //
                    responseText = "Файл принят на хранение.";                                                              //
                }                                                                                                           //
                else                                                                                                        //
                {                                                                                                           //
                    responseText = "Файл с таким названием уже находится в хранилище.";                                     //
                }                                                                                                           //
            }                                                                                                               //
            else if (update.Message.Type.ToString() == "Audio")/*-----------------------------------------------------------*/ //если тип полученного сообщения Audio
            {                                                                                                               //
                var fileID = update.Message.Audio.FileId;                                                                   //
                var fileNAME = update.Message.Audio.FileName;                                                               //
                var fromChat = update.Message.Chat.Id;                                                                      //
                var fromName = update.Message.From.Username;                                                                //
                                                                                                                            //
                if (Storage.CoincidenceCheck(fileNAME) == false)                                                            //
                {                                                                                                           //
                    Storage.AddNewFile(fileID, fileNAME, fromChat, fromName);                                               //
                                                                                                                            //
                    var file = await client.GetFileAsync(fileID);                                                           //
                                                                                                                            //
                    Console.WriteLine("FileNAME: " + fileNAME);                                                             //
                    Console.WriteLine("FilePath: " + file.FilePath + "\n");                                                 //
                                                                                                                            //
                    FileStream upload = new FileStream(storagePath + fileNAME, FileMode.Create);                            //
                    await client.DownloadFileAsync(file.FilePath, upload);                                                  //
                    upload.Close();                                                                                         //
                    responseText = "Файл принят на хранение.";                                                              //
                }                                                                                                           //
                else                                                                                                        //
                {                                                                                                           //
                    responseText = "Файл с таким названием уже находится в хранилище.";                                     //
                }                                                                                                           //
            }                                                                                                               //
            else if (update.Message.Type.ToString() == "Video")/*-----------------------------------------------------------*/ //если тип полученного сообщения Video
            {                                                                                                               //
                var fileID = update.Message.Video.FileId;                                                                   //
                var fileNAME = update.Message.Video.FileName;                                                               //
                var fromChat = update.Message.Chat.Id;                                                                      //
                var fromName = update.Message.From.Username;                                                                //
                                                                                                                            //
                if (Storage.CoincidenceCheck(fileNAME) == false)                                                            //
                {                                                                                                           //
                    Storage.AddNewFile(fileID, fileNAME, fromChat, fromName);                                               //
                                                                                                                            //
                    var file = await client.GetFileAsync(fileID);                                                           //
                                                                                                                            //
                    Console.WriteLine("FileNAME: " + fileNAME);                                                             //
                    Console.WriteLine("FilePath: " + file.FilePath + "\n");                                                 //
                                                                                                                            //
                    FileStream upload = new FileStream(storagePath + fileNAME, FileMode.Create);                            //
                    await client.DownloadFileAsync(file.FilePath, upload);                                                  //
                    upload.Close();                                                                                         //
                    responseText = "Файл принят на хранение.";                                                              //
                }                                                                                                           //
                else                                                                                                        //
                {                                                                                                           //
                    responseText = "Файл с таким названием уже находится в хранилище.";                                     //
                }                                                                                                           //
            }                                                                                                               //
            else/*----------------------------------------------------------------------------------------------------------*/ //если сообщение не относится к получаемым форматам файла
            {                                                                                                               //
                responseText = "Напишите /help для получения описания возможных комманд.";                                  //
            }                                                                                                               //
            /*--------------------------------------------------------------------------------------------------------------*/ //отправка текстовой реакции на сообщение
            await botClient.SendTextMessageAsync(update.Message.Chat, responseText);                                        //
            responseText = "";                                                                                              //

          //  Bot_Log_Processor("Add New Record", update);  // проблемы начинаются тут
          // BotLog.Add(new IncomingUpdate(
          //     update.Message.From.Id,
          //     update.Message.Date.ToString(),
          //     DateTime.Now.ToString(),
          //     update.Message.Type.ToString(),
          //     update.Message.Text,
          //     update.Message.From.FirstName,
          //     update.Message.From.Username
          //     ));
        }

        static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)  // обработчик ошибок
        {                                                                                                                           //
            if (exception is ApiRequestException apiRequestException)                                                               //
            {                                                                                                                       //
                await botClient.SendTextMessageAsync(123, apiRequestException.ToString());                                          //
            }                                                                                                                       //
        }
    }
}
