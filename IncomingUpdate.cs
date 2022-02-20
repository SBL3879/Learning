namespace Home_Work_10
{
    public struct IncomingUpdate
    {
        public long Id              { get; set; }
        public string SendDate      { get; set; }
        public string ReceiveDate   { get; set; }
        public string MsgType       { get; set; }
        public string MsgText       { get; set; }
        public string FirstName     { get; set; }
        public string NickName      { get; set; }

        public IncomingUpdate(long Id, string SendDate, string ReceiveDate, string MsgType, string MsgText, string FirstName, string NickName)
        {
            this.Id = Id;
            this.SendDate = SendDate;
            this.ReceiveDate = ReceiveDate;
            this.MsgType = MsgType;
            this.MsgText = MsgText;
            this.FirstName = FirstName;
            this.NickName = NickName;
        }
    }
}