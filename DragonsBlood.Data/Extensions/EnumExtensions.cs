using System;
using System.ComponentModel;

namespace DragonsBlood.Data.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDesc<T>(this string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if(attribute.Description.Trim().Replace(" ", "").ToString() == description)
                        return attribute.Description;
                }
                else
                {
                    if (field.Name == description)
                        return description;
                }
            }
            //throw new ArgumentException("Not found.", "description");
            return description;
        }
    }
}