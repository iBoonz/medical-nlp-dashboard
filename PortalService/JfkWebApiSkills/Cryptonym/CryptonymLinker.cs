using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MedicalNlpSkillsetFunctions.Cryptonym
{
    class CryptonymLinker
    {
        public CryptonymLinker(string executingDirectoryPath)
        {
            string json = File.ReadAllText($"{executingDirectoryPath}\\CryptonymLinker\\cia-cryptonyms.json");
            Cryptonyms = new Dictionary<string,string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(json), StringComparer.InvariantCultureIgnoreCase);
        }

        public Dictionary<string, string> Cryptonyms
        {
            get; private set;
        }
    }
}
