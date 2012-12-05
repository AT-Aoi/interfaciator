using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Utilities;
using ProjectManager.Projects;
using PluginCore.Managers;
using ASCompletion.Context;
using System.Collections;
using Interfaciator.Dialogs;
using PluginCore.Localization;
using System.Collections.Generic;
using ProjectManager.Controls.TreeView;

namespace Interfaciator
{
    public class PluginMain : IPlugin
    {
        #region Required fields
        private String pluginName = "Interfaciator";
        private String pluginGuid = "a2c159c1-7d21-4483-aeb1-38d9fdc4c7f3";
        private String pluginHelp = "http://flashtastic-fd-plugins.googlecode.com";
        private String pluginDesc = "Enables creation of interfaces based on an existing class.";
        private String pluginAuth = "Griz (http://www.flashtastic.ch)";
        private String settingFilename;
        private Settings settingObject;
        private Image pluginImage;
        #endregion

        #region Custom fields
        private String clickedPath;

        private String ifPackage;
        /// private String ifSuper;
        private String ifLang;
        private String oriPackage;
        private MemberModel[] ifGenMethods;
        private MemberList availableImports;

        private String fileCreated;

        private ToolStripItem tsi;
        private EventHandler tsi_ClickHandler;

        private static List<String> as3NativeClasses = new List<string>()
        {
            "ArgumentError",
            "arguments",
            "Array",
            "Boolean",
            "Class",
            "Date",
            "DefinitionError",
            "Error",
            "EvalError",
            "Function",
            "int",
            "JSON",
            "Math",
            "Namespace",
            "Number",
            "Object",
            "QName",
            "RangeError",
            "ReferenceError",
            "RegExp",
            "SecurityError",
            "String",
            "SyntaxError",
            "TypeError",
            "uint",
            "URIError",
            "Vector",
            "VerifyError",
            "XML",
            "XMLList"
        };

        public static IMainForm MainForm { get { return PluginBase.MainForm; } }
        #endregion

        #region Required Properties

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// Version of the API depending on the FD-version
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
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
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }

        #endregion

        #region Required methods

        public void Initialize()
        {
            InitBasics();
            LoadSettings();

            AddEventHandlers();
        }

        public void Dispose()
        {
            ///EventManager.RemoveEventHandler(this, EventType.Command | EventType.ProcessArgs);
            EventManager.RemoveEventHandler(this, EventType.FileSwitch, HandlingPriority.Low);
            this.SaveSettings();
        }

        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);
            EventManager.AddEventHandler(this, EventType.FileSwitch, HandlingPriority.Low);
            tsi.Click += tsi_ClickHandler;
        }

        #endregion

        #region Custom methods

        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "Interfaciator");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("100");

            tsi_ClickHandler = new EventHandler(tsi_Click);
            tsi = new ToolStripMenuItem("Extract Interface");
        }

        void tsi_Click(object sender, EventArgs e)
        {
            if (clickedPath.Length > 0)
            {
                FileModel fm = ASContext.Context.GetFileModel(clickedPath);
                fm.Check();
                availableImports = fm.Imports;                
                
                ClassModel cm = fm.GetPublicClass();

                String qn = cm.QualifiedName;
                oriPackage = qn.Substring(0, qn.LastIndexOf("."));

                Visibility v;
                FlagType ft;

                List<MemberModel> functions = new List<MemberModel>();

                foreach (MemberModel mm in cm.Members)
                {
                    v = mm.Access;
                    ft = mm.Flags;

                    if (mm.Name == cm.Name) ///Skip constructor
                        continue;
                    if ((v & Visibility.Public) > 0) ///only add if is public and...
                    {
                        if(((ft & FlagType.Function)|(ft & FlagType.Getter)|(ft & FlagType.Setter)) > 0) ///...is function, getter or setter
                        {
                            functions.Add(mm);
                        }
                    }
                }

                MethodPicker dialog = new MethodPicker();
                dialog.AutoSelect = settingObject.AutoSelectMethods;
                dialog.FullPath = settingObject.DisplayFullPath;
                dialog.Prefix = settingObject.NamePrefix;
                dialog.DisplayIcons = settingObject.DisplayIcons;
                dialog.DisplayModifiers = settingObject.DisplayModifiers;
                dialog.SourcePaths = getSourcePaths();
                dialog.SelectedFile = clickedPath;
                dialog.MethodList = functions.ToArray();

                dialog.FormClosing += new FormClosingEventHandler(methodPicker_FormClosing);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    String file = Path.ChangeExtension(dialog.SelectedFile, ".as");
                    String path = dialog.SelectedPath;

                    ifGenMethods = dialog.MethodList;
                    ifPackage = dialog.SelectedPackage;

                    ifLang = PluginBase.CurrentProject.Language;

                    generateFile(path, file);
                }

                dialog.FormClosing -= new FormClosingEventHandler(methodPicker_FormClosing);
            }
        }

        void methodPicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            MethodPicker mp = (MethodPicker)sender;
            if (mp.DialogResult != DialogResult.Cancel)
            {
                String path = mp.SelectedPath;
                String file = Path.ChangeExtension(mp.SelectedFile, ".as");
                String filePath = Path.Combine(path, file);

                if (File.Exists(filePath)) ///File already exists...
                {
                    string title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                    string message = TextHelper.GetString("ProjectManager.Info.FolderAlreadyContainsFile");

                    DialogResult result = MessageBox.Show(PluginBase.MainForm, string.Format(message, file, "\n"), title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
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

            String templateFolder = PathHelper.TemplateDir;
            String templatePath = Path.Combine("ProjectFiles", "AS3Project");
            templatePath = Path.Combine(templatePath, "Interface.as.fdt.wizard");
            String template = Path.Combine(templateFolder, templatePath);            

            if (!File.Exists(template)) ///Template doesn't exist...
            {
                templateFolder = Path.Combine(PathHelper.AppDir, "Templates");
                template = Path.Combine(templateFolder, templatePath);
                if (!File.Exists(template))
                {
                    ///Show Alert-Message that template-files are missing!
                    MessageBox.Show("Unfortunately it seems like you're lacking the template-files needed for processing. Go see https://bitbucket.org/Gr33z00/interfaciator/downloads to download the templates.");
                    return;
                }                
            }

            this.fileCreated = filePath;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            MainForm.FileFromTemplate(template, filePath);
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

        private string ProcessFileTemplate(String args)
        {
            Int32 eolMode = (Int32)MainForm.Settings.EOLMode;
            String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);

            List<String> imports = new List<String>();
            string inheritedMethods = "";

            /// generate functions
            /// TODO: change package of types that the interface gets placed aside
            foreach (MemberModel mm in ifGenMethods)
            {
                if ((mm.Flags & FlagType.Static) > 0)
                    inheritedMethods += "static ";

                inheritedMethods += "function ";

                if ((mm.Flags & FlagType.Getter) > 0 || (mm.Flags & FlagType.Setter) > 0)
                {
                    inheritedMethods += ((mm.Flags & FlagType.Getter) > 0) ? "get " : "set ";
                    inheritedMethods += mm.Name + "(" + mm.ParametersString(true) + "):" + mm.Type;
                }
                else
                {
                    inheritedMethods += mm.ToDeclarationString();
                }
                inheritedMethods += lineBreak + "\t\t";

                List<MemberModel> parameters = mm.Parameters;
                if (mm.Parameters == null)
                {
                    parameters = new List<MemberModel>();
                }
                parameters.Add(mm);

                Boolean found;
                String type;
                foreach (MemberModel parameter in parameters)
                {
                    type = parameter.Type;

                    if (type.ToLower().StartsWith("vector."))
                        continue;

                    found = false;
                    int dotPosition = parameter.Type.LastIndexOf(".");
                    if (dotPosition > -1)
                    {
                        imports.Add(type);
                    }
                    else
                    {
                        foreach (MemberModel import in availableImports)
                        {
                            if (import.Name.Equals(type))
                            {
                                imports.Add(import.Type);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            if (type.ToLower().Equals("void"))
                                continue;
                            if (as3NativeClasses.Contains(type))
                                continue;
                            if (oriPackage.Equals(ifPackage))
                                continue;

                            //assume type was aside of class...
                            imports.Add(oriPackage + "." + type);
                        }
                    }
                }
            }

            String importsSrc = "";
            String prevImport = null;
            imports.Sort();
            foreach (String import in imports)
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
            args = args.Replace("$(InheritedMethods)", inheritedMethods);
            return args;
        }

        #endregion

        #region IEventHandler Member

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
                                String key = TextHelper.GetString("ProjectManager.Label.CopyClassName");

                                int index = c.IndexOfKey(key);
                                if (index < 0)
                                {
                                    for (int i = 0; i < c.Count; i++)
                                    {
                                        if (c[i].Text.Equals(key))
                                        {
                                            index = i;
                                            break;
                                        }
                                    }
                                }
                                c.Insert(index+1, tsi);
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
