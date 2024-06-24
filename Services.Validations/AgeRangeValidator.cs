namespace Services.Validations
{
    public class AgeRangeValidator : IAgeRangeValidator
    {
        // Age range can be configured either in database or config file. 
        public bool IsRangeInValidRange(int userAge, int minAge, int maxAge)
            => userAge >= minAge && userAge <= maxAge;
    }
}
