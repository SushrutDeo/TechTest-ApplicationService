namespace Services.Validations
{
    public class MinimumPaymentValidator : IMinimumPaymentValidator
    {
        // Minimum payment can be configured either in database or config file. 
        public bool IsMinimumPaymentReceived(decimal recievedPayment, decimal minPayment)
            => recievedPayment >= minPayment;
    }
}
