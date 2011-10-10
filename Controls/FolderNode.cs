using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Interfaciator.Controls
{
    class FolderNode : TreeNode
    {
        protected String fullName;

        public FolderNode(String folderName, Boolean fullDisplay):this(folderName, fullDisplay, new String[0]) {}

        public FolderNode(String folderName, Boolean fullDisplay, String[] filteredNames)
        {
            fullName = folderName;
            if (!fullDisplay && filteredNames.Length > 0)
            {
                List<String> filtered = new List<string>(filteredNames.Length);
                filtered.AddRange(filteredNames);
                Stack<String> parts = new Stack<string>(folderName.Split(Path.DirectorySeparatorChar));
                folderName = parts.Pop();

                while ((folderName.Length < 1 || filtered.Contains(folderName)) && parts.Count > 0)
                    folderName = parts.Pop();
            }
            base.Text = folderName;
        }
    }
}
