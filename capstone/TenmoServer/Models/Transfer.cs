namespace TenmoServer.Models
{
    public class Transfer
    {


        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public double Balance { get; set; }
    }
}
