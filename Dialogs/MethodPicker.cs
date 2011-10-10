using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Model;
using System.Collections;
using System.IO;

namespace Interfaciator.Dialogs
{
    public partial class MethodPicker : Form
    {
        protected MemberModel[] members;
        protected Boolean autoSelect;
        protected Boolean fullPath;
        
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
                int added;
                for (int i = 0; i < members.Length; i++)
                {
                    mm = (MemberModel)members[i];
                    added = methodList.Items.Add(mm.ToDeclarationString());
                    methodList.SetItemChecked(added, autoSelect);
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

                //MessageBox.Show("extracted following path: '" + selectedPath + "' and name: '" + selectedName + "'");

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

        public MethodPicker(Boolean autoSelect, Boolean fullPath, String prefix) : this(autoSelect, fullPath, new String[0], prefix) { }

        public MethodPicker(Boolean autoSelect, Boolean fullPath, String[] filtered, String prefix)
        {
            this.namePrefix = prefix;
            this.fullPath = fullPath;
            this.autoSelect = autoSelect;
            this.filtered = filtered;
            InitializeComponent();
        }

        protected void selectPackage(String fullPackage)
        {
            String path;
            for (int i = 0; i < srcPaths.Length; i++)
            {
                path = srcPaths[i];
                //MessageBox.Show("Comparing " + path + " to " + fullPackage);
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
            //MessageBox.Show("found startIndex: " + start + " (computed: " + packagePath.IndexOf(srcPath) + ") for " + srcPath + " in " + packagePath);
            srcPath = (start != -1 && !packagePath.Equals(srcPath)) ? packagePath.Substring(start) : "";
            txtPackage.Text = srcPath.Replace(Path.DirectorySeparatorChar, '.');
        }

        private void MethodPicker_Load(object sender, EventArgs e)
        {
            this.btnOk.Focus();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CheckedListBox.CheckedItemCollection c = methodList.CheckedItems;
            ArrayList picked = new ArrayList();
            foreach(MemberModel m in members)
            {
                if (c.Contains(m.ToDeclarationString()))
                {
                    picked.Add(m);
                }
            }
            selectedPackage = txtPackage.Text;
            selectedName = txtName.Text;
            members = (MemberModel[])picked.ToArray(new MemberModel().GetType());
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
            for(int i = 0; i < this.methodList.Items.Count; i++)
            {
                this.methodList.SetItemChecked(i, true);
            }
        }

        private void btnDeselect_Click(object Sender, EventArgs e)
        {
            for (int i = 0; i < this.methodList.Items.Count; i++)
            {
                this.methodList.SetItemChecked(i, false);
            }
        }
    }
}
