using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class ProjectProperties : IProjectProperties
    {
        #region General

        string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                //OnPropertyChanged(nameof(Title));
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                // OnPropertyChanged(nameof(Description));
            }
        }

        string company;
        public string Company
        {
            get { return company; }
            set
            {
                company = value;
                // OnPropertyChanged(nameof(Company));
            }
        }

        string product;
        public string Product
        {
            get { return product; }
            set
            {
                product = value;
                //OnPropertyChanged(nameof(Product));
            }
        }

        string version;
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                //OnPropertyChanged(nameof(Version));
            }
        }

        #endregion General

        #region Build

        string buildOutputFolderPath = "!Output";
        public string BuildOutputFolderPath
        {
            get { return buildOutputFolderPath; }
            set
            {
                buildOutputFolderPath = value;
                //OnPropertyChanged(nameof(BuildOutputFolderPath));
            }
        }

        //set only for library
        string buildOutputFileName;
        public string BuildOutputFileName
        {
            get { return buildOutputFileName; }
            set
            {
                buildOutputFileName = value;
                //OnPropertyChanged(nameof(BuildOutputFileName));
            }
        }



        string buildOutputNamespace;
        public string BuildOutputNamespace
        {
            get { return buildOutputNamespace; }
            set
            {
                buildOutputNamespace = value;
                // OnPropertyChanged(nameof(BuildOutputNamespace));
            }
        }

        #endregion

        /// <summary>
        /// Project variables
        /// </summary>
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property> Properties { get; set; }
    }
}
