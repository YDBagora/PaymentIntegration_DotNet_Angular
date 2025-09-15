namespace CRUD_Application.Models
{
    public class RazorpayVerificationModel
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }
}
