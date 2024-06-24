namespace Services.Utilities
{
    public static class AgeCalculator
    {
        public static int CalculateAge(DateOnly dob)
        {
            int age = DateTime.Now.Year - dob.Year;

            if (dob.ToDateTime(TimeOnly.MinValue) > DateTime.Now.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
