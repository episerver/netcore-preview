
async function initAdyenCheckout(amount, currency) {
    try {
        // Calls your server endpoints
        async function callServer(url, data) {
            const res = await fetch(url, {
                method: "POST",
                body: data ? JSON.stringify(data) : "",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (res.status != 200) {
                $('.jsPaymentMethod').html("Payment method not configured correctly.");
            }

            return await res.json();
        }

        const paymentMethodsResponse = await callServer("/Payment/GetAdyenPaymentMethods", {
            merchantAccount: "",
        });

        const configuration = {
            paymentMethodsResponse: paymentMethodsResponse, // The `/paymentMethods` response from the server.
            clientKey: "test_N2H5WP7FMNGLBLETIP6VNAUIGIV4UBHA", // Web Drop-in versions before 3.10.1 use originKey instead of clientKey.
            locale: "en-US",
            environment: "test",
            onSubmit: (state, dropin) => {
                // Your function calling your server to make the `/payments` request
                if (state.isValid) {
                    submitCheckoutForm(state, dropin);
                }
            },
            paymentMethodsConfiguration: {
                card: { // Example optional configuration for Cards
                    hasHolderName: true,
                    holderNameRequired: true,
                    enableStoreDetails: true,
                    hideCVC: false, // Change this to true to hide the CVC field for stored cards
                    name: 'Credit or debit card',
                    amount: {
                        value: amount,
                        currency: currency
                    }
                }
            }
        };
        const checkout = new AdyenCheckout(configuration);
        const dropin = checkout.create('dropin').mount('#dropin-container');
    }
    catch (error) {
        console.log("these are errors", error)
    }
}

async function submitCheckoutForm(state, component) {
    var $form = $('.jsCheckoutForm');

    if ($("#BillingAddress_AddressId").val() == "") {
        document.getElementById('billingAddressContainer').scrollIntoView()
        $("#BillingAddressValidationMessage").html("Required");
        return;
    }

    var url = $form.attr('action');

    document.getElementById("additionalPaymentData[0].Key").value = "paymentData";
    document.getElementById("additionalPaymentData[0].Value").value = JSON.stringify(state.data);

    var data = $form.serialize();
    $.ajax({
        type: "POST",
        cache: false,
        url: url,
        data: data,
        success: function (result) {
            window.location = result;
        }
    });
}