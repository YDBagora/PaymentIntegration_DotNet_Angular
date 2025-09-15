using PayPal.Api;

namespace CRUD_Application.Services
{
    public class PayPalServices
    {
        private readonly IConfiguration _config;

        public PayPalServices(IConfiguration config)
        {
            _config = config;
        }

        private APIContext GetApiContext()
        {
            var clientId = _config["PayPal:ClientId"];
            var secret = _config["PayPal:Secret"];

            var config = new Dictionary<string, string>
            {
                {"mode", _config["PayPal:Mode"] }
            };

            var accessToken = new OAuthTokenCredential(clientId, secret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        public Payment CreatePayment(string baseUrl, string amount)
        {
            var apiContext = GetApiContext();

            var payer = new Payer() { payment_method = "paypal" };

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = $"{baseUrl}/cancel",
                return_url = $"{baseUrl}/success"
            };

            var amountObj = new Amount()
            {
                currency = "USD",
                total = amount
            };

            var transactionList = new List<Transaction>()
            {
                new Transaction()
                {
                    description = "Payment Transaction",
                    amount = amountObj
                }
            };

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirectUrls
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetApiContext();
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
