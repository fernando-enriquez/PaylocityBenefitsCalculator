namespace Api.Helpers
{
    public static class AgeUtils
    {
        // <summary>
        /// Calculates the age of a person as of January 1st of the current year.
        /// </summary>
        /// <param name="birthDate">The person's birth date.</param>
        /// <returns>The age in full years as of January 1st.</returns>
        public static int GetAgeAtStartOfYear(DateTime birthDate)
        {
            var referenceDate = new DateTime(DateTime.Today.Year, 1, 1);
            int age = referenceDate.Year - birthDate.Year;

            if (referenceDate < birthDate.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
