using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Interfaciator.Dialogs
{
    public partial class PathPicker : Form
    {
        protected String[] paths;
        protected String[] filtered;
        protected String chosenPath;
        protected Boolean fullPath;

        public PathPicker(Boolean fullPath, String[] filtered)
        {
            InitializeComponent();
            this.filtered = filtered;
            this.fullPath = fullPath;
            chosenPath = null;
        }

        private void treePaths_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode node = treePaths.SelectedNode;
        }


        public String ChosenPath {
            get { return chosenPath; }
            set {
                chosenPath = value;
                CheckForPath();
            }
        }

        public String[] SourcePaths
        {
            get { return paths; }
            set
            {
                paths = value;
                String path;
                for (int i = 0; i < paths.Length; i++)
                {
                    path = paths[i];
                    this.treePaths.Nodes.Add(CreateNode(path));
                }
                CheckForPath();
            }
        }

        protected void CheckForPath()
        {
            if (chosenPath == null || paths == null || paths.Length < 1 || treePaths == null)
                return;
            String path;
            for (int i = 0; i < paths.Length; i++)
            {
                path = paths[i];
                if (chosenPath.Contains(path))
                {
                    if (chosenPath.Equals(path))
                    {
                        treePaths.SelectedNode = treePaths.Nodes[i];
                        return;
                    }
                    else
                    {
                        path = chosenPath.Substring(path.Length);
                        if (OpenNode(new Queue<String>(path.Split(Path.DirectorySeparatorChar)), treePaths.Nodes[i]))
                            return;
                    }
                }
            }
        }

        protected Boolean OpenNode(Queue<String> src, TreeNode parent)
        {
            String nName = "";
            while (nName.Length == 0 && src.Count > 0)
            {
                nName = src.Dequeue();
            }
            if(nName.Length < 1)
                return false;
            TreeNode node;
            TreeNodeCollection nodes = parent.Nodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                node = nodes[i];
                if (node.Text.Equals(nName))
                {
                    if (src.Count == 0)
                    {
                        treePaths.SelectedNode = node;
                        return true;
                    }
                    else
                        OpenNode(src, node);
                }
            }
            return false;
        }

        protected TreeNode CreateNode(String src)
        {
            return CreateNode(src, false);
        }

        protected TreeNode CreateNode(String src, Boolean root)
        {
            DirectoryInfo di = new DirectoryInfo(src);
            String display = (this.fullPath && !root)? di.FullName : di.Name;
            ///TODO: Get filtered names from ProjectManager
            ///TreeNode tn = new FolderNode(display,fullPath,filtered);
            TreeNode tn = new TreeNode(display);
            tn.Name = (root)?di.Name : di.FullName;
            String[] children = Directory.GetDirectories(src);
            for (int i = 0; i < children.Length; i++)
            {
                tn.Nodes.Add(CreateNode(children[i], true));
            }
            return tn;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            TreeNode node = treePaths.SelectedNode;
            ///chosenPath = node.FullPath;
            chosenPath = GetFullPath(node);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetFullPath(TreeNode node)
        {
            String path = "";
            if (node.Parent != null)
            {
                path += GetFullPath(node.Parent) + Path.DirectorySeparatorChar;
            }
            path += node.Name;
            return path;
        }
    }
}
