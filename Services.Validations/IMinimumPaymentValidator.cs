namespace Services.Validations
{
    public interface IMinimumPaymentValidator
    {
        bool IsMinimumPaymentReceived(decimal recievedPayment, decimal minPayment);
    }
}