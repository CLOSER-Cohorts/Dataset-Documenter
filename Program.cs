using System;
using System.Collections.Generic;

using Algenta.Colectica.Commands.Import;
using Algenta.Colectica.Model;
using Algenta.Colectica.Model.Ddi;
using Algenta.Colectica.Model.Ddi.Serialization;
using Algenta.Colectica.Model.Utility;

namespace CLOSERDatasetDocumenter
{
    internal class Program
    {
        private const int ConfigFileColumns = 6;
        
        public static void Main(string[] args)
        {
            MultilingualString.CurrentCulture = "en-GB";
            VersionableBase.DefaultAgencyId = "uk.closer";
            
            if (args.Length < 1)
            {
                Logger.Instance.Log.ErrorFormat("CLOSER Dataset Documenter requires a config file.");
                Environment.Exit(1);
            }
            var configFile = args[0];

            try
            {
                var config = ValidateConfig(configFile);
                var spssImporter = new SpssImporter();
                var serializer = new Ddi32Serializer {UseConciseBoundedDescription = false};

                foreach (var record in config)
                {
                    Logger.Instance.Log.InfoFormat("Extracting metadata from {0}", record.Filename);

                    var resourcePackage = spssImporter.Import(record.Filename, record.Agency);
                    var instance = new DdiInstance();
                    instance.AgencyId = record.Agency;
                    instance.ResourcePackages.Add(resourcePackage);
                    var output = serializer.Serialize(instance);
                    output.Save(record.Scope + ".xml");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Log.ErrorFormat("Fatal error: {0}", e.Message);
            }
            
        }

        private static IEnumerable<Dataset> ValidateConfig(string configFile)
        {
            if (!System.IO.File.Exists(configFile))
            {
                throw new Exception("Config file could not be found.");
            }
            
            var records = new List<Dataset>();

            foreach (var line in System.IO.File.ReadAllLines(configFile))
            {
                // Skip lines beginning with a hash (comment)
                if (line.Trim()[0] == '#') continue;
                
                var pieces = line.Split('\t');
                if (pieces.Length != ConfigFileColumns)
                {
                    throw new Exception("Config file format error: " + line);
                }
                if (!System.IO.File.Exists(pieces[0]))
                {
                    throw new Exception("Could not find datafile: " + pieces[0]);
                }
                records.Add(new Dataset(pieces));;
            }

            return records;
        }
    }
}