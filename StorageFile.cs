namespace Home_Work_10
{
    struct StorageFile
    {
        public string FileID;   // id принятого на хранение файла
        public string FileNAME; // имя принятого на хранение файла
        public long FromChat; // id чата отправителя
        public string FromName; // имя отправителя

        public StorageFile(string FileID, string FileNAME, long FromChat, string FromName)
        {
            this.FileID = FileID;
            this.FileNAME = FileNAME;
            this.FromChat = FromChat;
            this.FromName = FromName;
        }
    }
}
