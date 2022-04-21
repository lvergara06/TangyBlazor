redirectToCheckout = function (sessionId) {
    var stripe = Stripe("pk_test_51KpgH3A2yC2td0V48mKay2zneOJ826Mxh72BAYzxlRWMrWxGUdyt5auq2qKm4vEhcOhvTSfYaPKJkkYZ9UAofB9w0014ZfqZR4");
    stripe.redirectToCheckout({ sessionId: sessionId });
}