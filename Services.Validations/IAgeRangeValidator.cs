namespace Services.Validations
{
    public interface IAgeRangeValidator
    {
        bool IsRangeInValidRange(int userAge, int minAge, int maxAge);
    }
}