<h1>Prerequisite</h1>
<p>Please register and take your publishable key and secret key at Stripe dashboard. (No need to activate payments.)</p>

![image](https://user-images.githubusercontent.com/34085190/200009038-723cdbf3-142a-4a68-a89f-f2eeeb71ce65.png)

<h1>Installation Step</h1>
<p>Install Stripe.net from Nuget Package Manager</p>
<h2>_Layout.cshtml</h2>
<p>Add these 3 CDN script tags.</p>

```html
<script src="https://js.stripe.com/v3/"></script>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.1/jquery.min.js"></script>
<script src="https://polyfill.io/v3/polyfill.min.js?version=3.52.1&features=fetch"></script>
```
<h2>PaymentIntentController.cs</h2>
<p><b>Notes: </b>Can be any name for the controller.</p>

```c#
    public class PaymentIntentApiController : Controller
    {
        public IActionResult Index()
        {
            // Set your secret key. Remember to switch to your live secret key in production.
            // See your keys here: https://dashboard.stripe.com/apikeys
            StripeConfiguration.ApiKey = "YOUR_SECRET_KEY";
            //Invoice bill
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1099,
                Currency = "myr",
                //Automatic payment method will use your allowed payment method set in Stripe account
                //Stripe doc have another method that allowed to set payment method manually
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            //Stripe will create a payment intent everytime checkout, it will return a "client secret" string, this string is like an unique id for each payment session.
            var service = new PaymentIntentService();
            var intent = service.Create(options);
            //Put in ViewData to show in the view
            ViewData["ClientSecret"] = intent.ClientSecret;
            return View();
        }
    }
```

<h2>Index.cshtml</h2>
<p>Hidden input form so that later can get the client secret.</p>

```cshtml
<input type="text" id="clientSecret" class="hidden"
       value="@ViewData["ClientSecret"]" />
```
<br>
<p>Just copy it, this is to show the card input form. (Better use it to follow the <a href="https://stripe.com/docs/security/guide#validating-pci-compliance">PCI Compliant</a>.)</p>

```cshtml
<form id="payment-form">
        <div id="payment-element">
            <!-- Elements will create form elements here -->
        </div>
        <button id="submit">Submit</button>
        <div id="error-message">
            <!-- Display error message to your customers here -->
        </div>
</form>
```
<br>
<p>Add script below it. Add your publishable key and return_url. "return_url" is the checkout success page.</p>

```cshtml
<script>
    //console.log(document.getElementById('clientSecret'));
    // This is your test publishable API key.
    const stripe = Stripe("YOUR_PUBLISHABLE_KEY");
    const options = {
        clientSecret: document.getElementById('clientSecret').value,
        // Fully customizable with appearance API.
        appearance: {/*...*/ },
    };
    // Set up Stripe.js and Elements to use in checkout form, passing the client secret obtained in step 3
    const elements = stripe.elements(options);
    // Create and mount the Payment Element
    const paymentElement = elements.create('payment', {
        layout: {
            type: 'tabs',
            defaultCollapsed: false,
        }
    });
    paymentElement.mount('#payment-element');
    const form = document.getElementById('payment-form');
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const { error } = await stripe.confirmPayment({
            //`Elements` instance that was used to create the Payment Element
            elements,
            confirmParams: {
                return_url: 'https://localhost:7267/PaymentIntentApi/Success',
            },
        });
        if (error) {
            // This point will only be reached if there is an immediate error when
            // confirming the payment. Show error to your customer (for example, payment
            // details incomplete)
            const messageContainer = document.querySelector('#error-message');
            messageContainer.textContent = error.message;
        } else {
            // Your customer will be redirected to your `return_url`. For some payment
            // methods like iDEAL, your customer will be redirected to an intermediate
            // site first to authorize the payment, then redirected to the `return_url`.
        }
    });
</script>
```

<h2>Success.cshtml</h2>
<p>Create a checkout success page.</p>
<p>For example: </p>

```cshtml
<section>
    <p>
        Purchase Success! Your parcel will be prepared and send out.
        <button type="button" onclick="location.href='@Url.Action("Index","Home")'">Back to Home</button>
    </p>
</section>
```

<h1>Result Interface</h1>
<p>At the "Payment" tab:</p>

![image](https://user-images.githubusercontent.com/34085190/200014892-d1d651ef-cd61-49c9-81fa-b1c1b0e4f878.png)

<h1>Test Card Number</h1>
<p><b>Card Number: <b/>4242 4242 4242 4242</p>
<p>Other information is random number.</p>

<h1>Useful Link</h1>
- <a href="https://stripe.com/docs/testing#cards">All Test Card Number</a>
<br>
- <a href="https://stripe.com/docs/payments/quickstart">Custom Payment Flow Integration</a>
<br>
<p><b>Notes: </b>Please click the custom payment flow here.</p>

![image](https://user-images.githubusercontent.com/34085190/200021374-ed4cc9f0-b681-4c08-99ee-3060c91c1591.png)
