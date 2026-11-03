using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Exceptions;
namespace HelpDesk.Application.Common;

public static class Validation
{
    public static void Check(object input)
    {
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(input, new ValidationContext(input), errors, true))
            throw new BusinessException(400, string.Join(" ", errors.Select(x => x.ErrorMessage)));
        foreach (var property in input.GetType().GetProperties())
            if (property.PropertyType == typeof(Guid) && (Guid)property.GetValue(input)! == Guid.Empty)
                throw new BusinessException(400, $"{property.Name} debe ser un identificador valido.");
    }
    public static void Require(bool condition, string message, int status = 400)
    {
        if (!condition) throw new BusinessException(status, message);
    }
}
