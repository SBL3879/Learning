using System.Collections.Generic;
using System.IO;

namespace Home_Work_10
{
    class Repository
    {
        public List<StorageFile> files_storage = new List<StorageFile>();
        string indexFilePath = BotControll.storagePath + "Indexes.txt";
        string[] DataToSave; // массив данных для записи в файл

        public Repository() // создание репозитория из сохранённого файла
        {
            if (File.Exists(indexFilePath))
            {
                string[] indexes = File.ReadAllLines(indexFilePath);

                for (int i = 0; i < indexes.Length; i++)
                {
                    string[] stringParts = indexes[i].Split(':');
                    files_storage.Add(new StorageFile(stringParts[0], stringParts[1], long.Parse(stringParts[2]), stringParts[3]));
                }
            }
            else
            {
                File.Create(indexFilePath);
            }
        }

        public void AddNewFile(string FileID, string FileNAME, long FromChat, string FromName) // добавление нового файла
        {
            files_storage.Add(new StorageFile(FileID, FileNAME, FromChat, FromName));

            DataToSave = new string[files_storage.Count];
            for (int i = 0; i < DataToSave.Length; i++)
            {
                DataToSave[i] = 
                    files_storage[i].FileID + ":" + 
                    files_storage[i].FileNAME + ":" + 
                    files_storage[i].FromChat + ":" + 
                    files_storage[i].FromName;
            }

            File.WriteAllLines(indexFilePath, DataToSave);
        }

        public bool CoincidenceCheck(string name) // проверка совпадений
        {
            bool YesOrNot = false;

            string[] indexes = File.ReadAllLines(indexFilePath);

            foreach (string index in indexes)
            {
                string[] splitedString = index.Split(':');
                if (splitedString[1] == name) YesOrNot = true;
            }

            return YesOrNot;
        }
    }
}
