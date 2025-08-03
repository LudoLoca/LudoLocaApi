using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Backend.Helpers;

public static class EnumExtensions
{
    // Retorna o valor do atributo [Display(Name = ...)] ou o nome do enum se não houver
    public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : Enum
    {
        var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
    }
}
