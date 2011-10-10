using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace Interfaciator
{
    [Serializable]
    public class Settings
    {
        const Boolean AUTOSELECT_METHODS = true;
        protected Boolean autoSelectMethods = AUTOSELECT_METHODS;

        [DisplayName("Method auto-selection")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Determines if all the methods in the list should be chosen by default or not"), DefaultValue(AUTOSELECT_METHODS)]
        public Boolean AutoSelectMethods
        {
            get { return autoSelectMethods; }
            set { autoSelectMethods = value; }
        }

        const Boolean SHOW_FULLPATH = false;
        protected Boolean showFullPath = SHOW_FULLPATH;

        [DisplayName("Show full path")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Determines if the full path is displayed in path-picker"), DefaultValue(SHOW_FULLPATH)]
        public Boolean ShowFullPath
        {
            get { return showFullPath; }
            set { showFullPath = value; }
        }

        /*const Boolean INCLUDE_INTERFACES = true;
        protected Boolean includeInterfaces = INCLUDE_INTERFACES;

        [DisplayName("Include Interfaces")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Determines if option should be allowed in interfaces"), DefaultValue(INCLUDE_INTERFACES)]
        public Boolean IncludeInterfaces
        {
            get { return includeInterfaces; }
            set { includeInterfaces = value; }
        }///*/

        const String NAME_PREFIX = "I";
        protected String namePrefix = NAME_PREFIX;

        [DisplayName("Name prefix")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Defines the prefix for the name of the interface to be created"), DefaultValue(NAME_PREFIX)]
        public String NamePrefix
        {
            get { return namePrefix; }
            set { namePrefix = value; }
        }

        /*
        protected string[] filteredDirectoryNames = new string[] { "src", "source", "sources", "as", "as2", "as3", "actionscript", "flash", "classes", "trunk", "svn" };

        [DisplayName("Filtered Directory Names")]
        [LocalizedDescription("ProjectManager.Description.FilteredDirectoryNames")]
        [LocalizedCategory("ProjectManager.Category.Exclusions")]
        public string[] FilteredDirectoryNames
        {
            get { return filteredDirectoryNames; }
            set { filteredDirectoryNames = value; }
        }///*/
    }
}