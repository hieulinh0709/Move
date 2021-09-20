using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using PluginValidator.Configurations;
using System;
using System.IO;

namespace PluginValidator.FileHandler
{
    public class ReadOnlyJson
    {
        public PhoneConfiguration ReadJson(string path)
        {
            using (StreamReader Filejosn = new StreamReader(path))
            {
                try
                {
                    var json = Filejosn.ReadToEnd();
                    PhoneConfiguration phoneConfiguration = JsonConvert.DeserializeObject<PhoneConfiguration>(json);

                    return phoneConfiguration;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem reading file: " + ex);

                    throw new InvalidPluginExecutionException("Problem reading file: " + ex);
                }
            }

        }
    }
}
