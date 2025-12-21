using System.Globalization;
using System.Windows.Controls;

namespace MagazAvtoZap.Views
{
    public class PositiveIntegerRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Поле обязательно для заполнения.");
            }

            if (!int.TryParse(value.ToString(), out int number))
            {
                return new ValidationResult(false, "Значение должно быть целым числом.");
            }

            if (number <= 0)
            {
                return new ValidationResult(false, "Значение должно быть больше 0.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
