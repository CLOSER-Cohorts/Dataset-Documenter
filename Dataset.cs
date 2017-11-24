using System;

namespace CLOSERDatasetDocumenter
{
    public class Dataset
    {
        public string Filename;
        public string Scope;
        public string Agency;
        public bool IsPublic;
        public string Doi;
        public string Options;

        public Dataset(string[] pieces)
        {
            Filename = pieces[0];
            Scope = pieces[1];
            Agency = pieces[2];
            IsPublic = Convert.ToBoolean(Convert.ToInt32(pieces[3]));
            Doi = pieces[4];
            Options = pieces[5];
        }
    }
}