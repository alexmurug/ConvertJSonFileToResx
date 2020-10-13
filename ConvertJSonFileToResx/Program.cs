using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConvertJSonFileToResx
{
    class Program
    {
        public static IConfigurationRoot configuration;
        static void Main()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Read the file and display it line by line.  
            StreamReader jsonFile = new StreamReader(configuration.GetSection("sourcePath").Value);

            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFile.ReadToEnd());

            using (StreamWriter file = new StreamWriter(configuration.GetSection("savePath").Value))
                foreach (var entry in values)
                    file.WriteLine($@"<data name=""{entry.Key}"" xml:space=""preserve"">" +
                        $"\n\t<value>{entry.Value}</value>\n" +
                        $"</data>");

            using (StreamWriter file = new StreamWriter(configuration.GetSection("saveKeysForWebConfigPath").Value))
                foreach (var entry in values)
                    file.Write($@"{entry.Key},");



        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(configuration);

            // Add app
            serviceCollection.AddTransient<Program>();
        }
    }
}
