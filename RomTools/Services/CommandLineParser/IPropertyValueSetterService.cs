using System.Reflection;

namespace RomTools.Services.CommandLineParser;

public interface IPropertyValueSetterService
{
    bool SetPropertyValue<T>(
        T option,
        PropertyInfo property,
        string value);
}
