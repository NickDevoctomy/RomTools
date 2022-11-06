using System.Text;

namespace RomTools.Services.CommandLineParser
{
    public class ArgumentsFlattenerService : IArgumentsFlattenerService
    {
        public string Flatten(string[] arguments)
        {
            var flat = new StringBuilder();
            foreach(var curArgument in arguments)
            {
                var equalsLocation = curArgument.IndexOf('=');
                if(equalsLocation > 0)
                {
                    var token = curArgument.Substring(0, equalsLocation);
                    flat.Append(token);
                    flat.Append('=');
                    var value = curArgument.Substring(equalsLocation + 1);
                    flat.Append(value.Contains(" ") ? $"\"{value}\"" : $"{value}");
                }
                else
                {
                    flat.Append(curArgument);                    
                }
                flat.Append(' ');
            }

            return flat.ToString().TrimEnd();
        }
    }
}
