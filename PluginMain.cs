using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using Interfaciator.Dialogs;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager.Controls.TreeView;
using ProjectManager.Projects;

namespace Interfaciator
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "Interfaciator";
        private String pluginGuid = "a2c159c1-7d21-4483-aeb1-38d9fdc4c7f3";
        private String pluginHelp = "http://flashtastic-fd-plugins.googlecode.com";
        private String pluginDesc = "Enables creation of interfaces based on an existing class.";
        private String pluginAuth = "Griz (http://www.flashtastic.ch)";

        private String settingFilename;
        private Settings settingObject;

        private String clickedPath;

        private String ifPackage;
        /// private String ifSuper;
        private String ifLang;
        private MemberModel[] ifGenMethods;

        private String fileCreated;

        private ToolStripItem tsi;
        private EventHandler tsi_ClickHandler;

        public static IMainForm MainForm { get { return PluginBase.MainForm; } }

        #region Required Properties

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return this.pluginGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        public Object Settings
        {
            get { return this.settingObject; }
        }

        #endregion

        #region IPlugin Member

        public void Dispose()
        {
            this.SaveSettings();
        }

        public void Initialize()
        {
            InitBasics();
            LoadSettings();

            AddEventHandlers();
        }

        #endregion

        #region IEventHandler Member

        private void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "Interfaciator");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");

            tsi_ClickHandler = new EventHandler(tsi_Click);
            tsi = new ToolStripMenuItem("Extract Interface");
            tsi.Click += tsi_ClickHandler;
        }

        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);
            EventManager.AddEventHandler(this, EventType.FileSwitch, HandlingPriority.Low);
        }

        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            ///Project project = PluginBase.CurrentProject as Project;

            switch (e.Type)
            {
                case EventType.Command:

                    DataEvent evt = (DataEvent)e;
                    if (evt.Action == "ProjectManager.TreeSelectionChanged")
                    {
                        ProjectTreeView ptv = sender as ProjectTreeView;
                        GenericNode n = ptv.SelectedNode;
                        if (n is FileNode)
                        {
                            string path = n.BackingPath;
                            string ext = Path.GetExtension(path);

                            if (FileInspector.IsActionScript(path, ext))
                            {
                                clickedPath = path;
                                ToolStripItemCollection c = ptv.ContextMenuStrip.Items;
                                c.Insert(7, tsi);
                            }
                        }
                    }
                    break;
                case EventType.FileSwitch:
                    fileCreated = null;
                    ifGenMethods = null;
                    break;
                case EventType.ProcessArgs:
                    TextEvent te = e as TextEvent;
                    Project project = PluginBase.CurrentProject as Project;
                    if (fileCreated != null && project != null && (project.Language.StartsWith("as")))
                    {
                        te.Value = ProcessArgs(project, te.Value);
                    }
                    break;
            }
        }

        void tsi_Click(object sender, EventArgs e)
        {
            /*ToolStripItem tsi = (ToolStripItem)sender;
            tsi.Click -= tsi_ClickHandler;*/
            if (clickedPath.Length > 0)
            {
                FileModel fm = ASContext.Context.GetFileModel(clickedPath);
                fm.Check();

                ClassModel cm = fm.GetPublicClass();
                Visibility v;

                ArrayList functions = new ArrayList();

                uint function;
                uint getter;
                uint setter;
                uint valid;

                foreach (MemberModel mm in cm.Members)
                {
                    v = mm.Access;
                    FlagType ft = mm.Flags;

                    if (mm.Name == cm.Name) ///Skip constructor
                        continue;
                    if (v.Equals(Visibility.Public)) ///only add if is public and...
                    {
                        function = ((uint)ft & (1 << 19));
                        getter = ((uint)ft & (1 << 20));
                        setter = ((uint)ft & (1 << 21));
                        valid = function + getter + setter;

                        if (valid > 0)    ///... is a function, a getter or a setter
                        {
                            functions.Add(mm);
                        }
                    }
                }

                ///MethodPicker dialog = new MethodPicker(this.settingObject.AutoSelectMethods, this.settingObject.ShowFullPath, this.settingObject.FilteredDirectoryNames, this.settingObject.NamePrefix);
                MethodPicker dialog = new MethodPicker(this.settingObject.AutoSelectMethods, this.settingObject.ShowFullPath, this.settingObject.NamePrefix);
                dialog.SourcePaths = getSourcePaths();
                dialog.SelectedFile = clickedPath;
                dialog.MethodList = (MemberModel[])functions.ToArray(new MemberModel().GetType());

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    String file = Path.ChangeExtension(dialog.SelectedFile, ".as");
                    String path = dialog.SelectedPath;

                    ifGenMethods = dialog.MethodList;
                    ifPackage = dialog.SelectedPackage;

                    ifLang = PluginBase.CurrentProject.Language;

                    generateFile(path, file);
                }
                else
                {
                    dialog.Hide();
                }
            }
        }

        protected String[] getSourcePaths()
        {
            String[] raw = PluginBase.CurrentProject.SourcePaths;
            String[] ok = new String[raw.Length];
            String basePath = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
            for (int i = 0; i < raw.Length; i++)
            {
                ok[i] = Path.Combine(basePath, raw[i]);
            }
            return ok;
        }

        protected void generateFile(String path, String file)
        {
            string filePath = Path.Combine(path, file);

            if (File.Exists(filePath)) ///File already exists...
            {
                string title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string message = TextHelper.GetString("ProjectManager.Info.FolderAlreadyContainsFile");
                DialogResult result = MessageBox.Show(PluginBase.MainForm, string.Format(message, file, "\n"), title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
            }

            String templPath = Path.Combine(PathHelper.TemplateDir, "ProjectFiles");
            templPath = Path.Combine(templPath, "AS3Project");
            String templatePath = Path.Combine(templPath, "Interface.as.fdt.wizard");
            if (!File.Exists(templatePath)) ///Template doesn't exist...
                return;

            this.fileCreated = filePath;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            MainForm.FileFromTemplate(templatePath, filePath);
        }

        public string ProcessArgs(Project project, string args)
        {
            if (fileCreated != null)
            {
                string package = ifPackage != null ? ifPackage : "";

                string fileName = Path.GetFileNameWithoutExtension(fileCreated);
                args = args.Replace("$(FileName)", fileName);

                if (args.Contains("$(FileNameWithPackage)") || args.Contains("$(Package)"))
                {
                    args = args.Replace("$(Package)", package);
                    if (package != "") args = args.Replace("$(FileNameWithPackage)", package + "." + fileName);
                    else args = args.Replace("$(FileNameWithPackage)", fileName);

                    if (ifPackage != null)
                    {
                        args = ProcessFileTemplate(args);
                        ifPackage = null;
                    }
                }
                fileCreated = null;
            }
            return args;
        }

        private string ProcessFileTemplate(string args)
        {
            Int32 eolMode = (Int32)MainForm.Settings.EOLMode;
            String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);

            List<String> imports = new List<string>();
            ///string extends = "";
            string inheritedMethods = "";
            
            /// generate functions
            foreach (MemberModel mm in ifGenMethods)
            {
                if((mm.Flags & FlagType.Static) > 0)
                    inheritedMethods += "static ";

                if ((mm.Flags & FlagType.Getter) > 0 || (mm.Flags & FlagType.Setter) > 0 )
                {
                    inheritedMethods += "function ";
                    inheritedMethods += ((mm.Flags & FlagType.Getter) > 0) ? "get " : "set ";
                    inheritedMethods += mm.Name + "(" + mm.ParametersString(true) + "):" + mm.Type + lineBreak + "\t\t";
                    continue;
                }


                inheritedMethods += "function " + mm.ToDeclarationString() + lineBreak + "\t\t";
            }


            // resolve extension and import
            /*if (ifSuper != null && ifSuper != "")
            {
                string[] _extends = ifSuper.Split('.');
                if (_extends.Length > 1)
                    imports.Add(ifSuper);

                ///extends = " extends " + _extends[_extends.Length - 1];
            }*/

            string importsSrc = "";
            string prevImport = null;
            imports.Sort();
            foreach (string import in imports)
            {
                if (prevImport != import)
                {
                    prevImport = import;
                    if (import.LastIndexOf('.') == -1) continue;
                    if (import.Substring(0, import.LastIndexOf('.')) == ifPackage) continue;
                    importsSrc += (ifLang == "as3" ? "\t" : "") + "import " + import + ";" + lineBreak;
                }
            }
            if (importsSrc.Length > 0)
            {
                importsSrc += (ifLang == "as3" ? "\t" : "") + lineBreak;
            }

            args = args.Replace("$(Import)", importsSrc);
            ///args = args.Replace("$(Extends)", extends);
            args = args.Replace("$(InheritedMethods)", inheritedMethods);
            return args;
        }

        #endregion

        #region Settings

        private void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        private void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        #endregion
    }
}
