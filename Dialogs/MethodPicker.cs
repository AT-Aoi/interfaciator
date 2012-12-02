using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using ASCompletion.Model;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using System.Reflection;
using Interfaciator.Controls;
using System.Collections.Generic;

namespace Interfaciator.Dialogs
{
    public partial class MethodPicker : Form
    {
        protected MemberModel[] members;
        protected Boolean autoSelect;
        protected Boolean fullPath;
        protected Boolean showIcons;
        protected Boolean showModifiers;
        
        protected String selectedPath;
        protected String selectedName;
        protected String selectedPackage;
        protected String namePrefix;
        protected String[] srcPaths;
        protected String[] filtered;

        public MemberModel[] MethodList
        {
            get { return members; }
            set
            {
                members = value;
                MemberModel mm;
                int icon;
                FlagType ft;
                List<string> modifiers;
                for (int i = 0; i < members.Length; i++)
                {
                    mm = (MemberModel)members[i];
                    modifiers = new List<string>();
                    ft = mm.Flags;

                    if (showIcons)
                    {
                        icon = (((ft & FlagType.Getter) | (ft & FlagType.Setter)) > 0) ? 1 : 0;
                    }
                    else
                    {
                        icon = -1;
                    }
                    if (showModifiers)
                    {
                        if ((ft & FlagType.Static) > 0)
                        {
                            modifiers.Add("static");
                        }
                        if ((ft & FlagType.Getter) > 0)
                        {
                            modifiers.Add("get");
                        }
                        else if ((ft & FlagType.Setter) > 0)
                        {
                            modifiers.Add("set");
                        }
                    }
                    methodList.Items.Add(new GListBox.GListBoxItem(mm.ToDeclarationString(), modifiers.ToArray(), icon));
                }
                if (autoSelect)
                {
                    methodList.SelectAll();
                }
            }
        }

        public String SelectedFile
        {
            get { return selectedName; }
            set
            {
                String path = value;
                int index = path.LastIndexOf(Path.DirectorySeparatorChar);
                selectedPath = (index != -1) ? path.Substring(0, index) : "";
                selectedName = (index != -1) ? path.Substring(index + 1) : path;
                index = selectedName.LastIndexOf(".");
                selectedName = (index != -1) ? selectedName.Substring(0, index) : selectedName;

                txtName.Text = namePrefix + selectedName;
                if (srcPaths != null)
                    selectPackage(selectedPath);
            }
        }

        public String SelectedPath
        {
            get { return selectedPath; }
            set
            {
                selectedPath = value;
            }
        }

        public String SelectedPackage
        {
            get { return selectedPackage; }
            set {
                selectedPackage = value;
                txtPackage.Text = selectedPackage;
            }
        }

        public String[] SourcePaths
        {
            get { return srcPaths; }
            set
            {
                srcPaths = new String[value.Length];
                String path;
                for (int i = 0; i < value.Length; i++)
                {
                    path = value[i];
                    path = Path.GetFullPath(path);
                    srcPaths[i] = path;
                }
                if (selectedName != null)
                    selectPackage(selectedPath);
            }
        }

        public Boolean AutoSelect
        {
            get { return autoSelect; }
            set { autoSelect = value; }
        }

        public Boolean FullPath
        {
            get { return fullPath; }
            set { fullPath = value; }
        }

        public string Prefix
        {
            get { return namePrefix; }
            set { namePrefix = value; }
        }

        public Boolean DisplayIcons
        {
            get { return showIcons; }
            set { showIcons = value; }
        }

        public Boolean DisplayModifiers
        {
            get { return showModifiers; }
            set { showModifiers = value; }
        }

        public MethodPicker() : this(false, false, "") { }

        public MethodPicker(Boolean autoSelect, Boolean fullPath, String prefix) : this(autoSelect, fullPath, new String[0], prefix) { }

        public MethodPicker(Boolean autoSelect, Boolean fullPath, String[] filtered, String prefix)
        {
            this.namePrefix = prefix;
            this.fullPath = fullPath;
            this.autoSelect = autoSelect;
            this.filtered = filtered;

            InitializeComponent();
            InitializeImages();
        }

        private void InitializeImages()
        {
            ImageList icons = new ImageList();
            Assembly self = Assembly.GetExecutingAssembly();

            icons.Images.Add(Properties.Resources.method);
            icons.Images.Add(Properties.Resources.property);
            
            this.methodList.ImageList = icons;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.Cancel)
            {
                String path = this.selectedPath;
                String file = Path.ChangeExtension(this.SelectedFile, ".as");
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

        protected void selectPackage(String fullPackage)
        {
            String path;
            for (int i = 0; i < srcPaths.Length; i++)
            {
                path = srcPaths[i];
                if (fullPackage.Contains(path))
                {
                    showPackage(fullPackage, path);
                    break;
                }
            }
        }

        protected void showPackage(String packagePath, String srcPath)
        {
            int start = packagePath.IndexOf(srcPath) + srcPath.Length + 1;
            srcPath = (start != -1 && !packagePath.Equals(srcPath)) ? packagePath.Substring(start) : "";
            txtPackage.Text = srcPath.Replace(Path.DirectorySeparatorChar, '.');
        }

        private void MethodPicker_Load(object sender, EventArgs e)
        {
            this.btnOk.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection c = methodList.SelectedItems;

            List<MemberModel> picked = new List<MemberModel>();
            foreach (GListBox.GListBoxItem item in c)
            {
                foreach (MemberModel m in members)
                {
                    if (m.ToDeclarationString().Equals(item.Text))
                    {
                        picked.Add(m);
                        break;
                    }
                }
            }
            selectedPackage = txtPackage.Text;
            selectedName = txtName.Text;
            members = picked.ToArray();
            this.DialogResult = DialogResult.OK;
        }

        private void btnSearch_Click(object Sender, EventArgs e)
        {
            PathPicker pp = new PathPicker(fullPath,filtered);
            pp.ChosenPath = selectedPath;
            pp.SourcePaths = srcPaths;
            if (pp.ShowDialog() == DialogResult.OK)
            {
                selectedPath = pp.ChosenPath;
                selectPackage(selectedPath);
            }
        }

        private void btnSelect_Click(object Sender, EventArgs e)
        {
            this.methodList.SelectAll();
            /*for(int i = 0; i < this.methodList.Items.Count; i++)
            {
                ///this.methodList.SetItemChecked(i, true);
                this.methodList.SetSelected(i, true);
            }*/
        }

        private void btnDeselect_Click(object Sender, EventArgs e)
        {
            this.methodList.DeselectAll();
            /*for (int i = 0; i < this.methodList.Items.Count; i++)
            {
                ///this.methodList.SetItemChecked(i, false);
                this.methodList.SetSelected(i, false);
            }*/
        }
    }
}
