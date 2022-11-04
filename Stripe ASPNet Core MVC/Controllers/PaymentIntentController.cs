using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Stripe_ASPNet_Core_MVC.Controllers
{
    public class PaymentIntentApiController : Controller
    {
        public IActionResult Index()
        {
            // Set your secret key. Remember to switch to your live secret key in production.
            // See your keys here: https://dashboard.stripe.com/apikeys
            StripeConfiguration.ApiKey = "YOUR_SECRET_KEY";

            //invoice bill
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1099,
                Currency = "myr",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
            var intent = service.Create(options);

            ViewData["ClientSecret"] = intent.ClientSecret;
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
