using Razorpay.Api;

namespace CRUD_Application.Services
{
    public class RazorpayService
    {
        private readonly string key = "rzp_test_RGhzCZg731Q1gy";
        private readonly string secret = "dSkBETwnP0JQk3wQ1KDiNuMw";

        public void VerifyPayment(string orderId, string paymentId, string signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>()
            {
                  { "razorpay_order_id", orderId },
                  { "razorpay_payment_id", paymentId },
                  { "razorpay_signature", signature }
            };

            Utils.verifyPaymentSignature(attributes);
        }
    }
}
    