using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace Interfaciator
{
    [Serializable]
    public class Settings
    {
        const Boolean AUTOSELECT_METHODS = false;
        protected Boolean autoSelectMethods = AUTOSELECT_METHODS;

        [DisplayName("Method auto-selection")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Determines if all the methods in the list should be chosen by default or not"), DefaultValue(AUTOSELECT_METHODS)]
        public Boolean AutoSelectMethods
        {
            get { return autoSelectMethods; }
            set { autoSelectMethods = value; }
        }

        const Boolean DISPLAY_FULLPATH = false;
        protected Boolean displayFullPath = DISPLAY_FULLPATH;

        [DisplayName("Display full path")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Determines if the full path is displayed in path-picker"), DefaultValue(DISPLAY_FULLPATH)]
        public Boolean DisplayFullPath
        {
            get { return displayFullPath; }
            set { displayFullPath = value; }
        }

        const Boolean DISPLAY_ICONS = true;
        protected Boolean displayIcons = DISPLAY_ICONS;

        [DisplayName("Display icons")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Defines if type-icons are displayed in pick-dialog"), DefaultValue(DISPLAY_ICONS)]
        public Boolean DisplayIcons
        {
            get { return displayIcons; }
            set { displayIcons = value; }
        }

        const Boolean DISPLAY_MODIFIERS = true;
        protected Boolean displayModifiers = DISPLAY_MODIFIERS;

        [DisplayName("Display modifiers")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Defines if modifiers (get/set, static) are displayed in pick-dialog"), DefaultValue(DISPLAY_MODIFIERS)]
        public Boolean DisplayModifiers
        {
            get { return displayModifiers; }
            set { displayModifiers = value; }
        }

        const String NAME_PREFIX = "I";
        protected String namePrefix = NAME_PREFIX;

        [DisplayName("Name prefix")]
        [LocalizedCategory("ASCompletion.Category.Common"), Description("Defines the prefix for the name of the interface to be created"), DefaultValue(NAME_PREFIX)]
        public String NamePrefix
        {
            get { return namePrefix; }
            set { namePrefix = value; }
        }
    }
}