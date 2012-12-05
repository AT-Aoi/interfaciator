using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace Interfaciator.Controls
{
    // GListBox class 
    public class GListBox : ListBox
    {
        private ImageList _myImageList;
        public ImageList ImageList
        {
            get { return _myImageList; }
            set { _myImageList = value; }
        }
        public GListBox()
        {
            // Set owner draw mode
            this.ItemHeight = this.Font.Height + 2;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.SelectedIndexChanged += new EventHandler(GListBox_SelectedIndexChanged);
        }

        void GListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        public void SelectAll()
        {
            for(int i = 0; i < this.Items.Count; i++)
            {
                this.SetSelected(i, true);
            }
        }

        public void DeselectAll()
        {
            this.ClearSelected();
        }

        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            GListBoxItem item;
            Rectangle bounds = e.Bounds;
            Size imageSize = (_myImageList != null)?_myImageList.ImageSize:new Size(48,48);
            int left = bounds.Left;

            try
            {
                item = (GListBoxItem)Items[e.Index];

                CheckBoxState cbState = this.SelectedItems.Contains(item)?CheckBoxState.CheckedNormal:CheckBoxState.UncheckedNormal;
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(left, bounds.Top), cbState);
                left += imageSize.Width;

                if (item.ImageIndex != -1)
                {
                    ImageList.Draw(e.Graphics, left, bounds.Top, item.ImageIndex);
                    left += imageSize.Width;  
                }
                e.Graphics.DrawString(item.Label, e.Font, new SolidBrush(e.ForeColor),
                       left, bounds.Top);
            }
            catch
            {
                if (e.Index != -1)
                {
                    if (Items.Count > 0 && Items.Count >= e.Index + 1)
                    {
                        e.Graphics.DrawString(Items[e.Index].ToString(), e.Font,
                            new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
                    }
                }
                else
                {
                    e.Graphics.DrawString(Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left, bounds.Top);
                }
            }
            base.OnDrawItem(e);
        }


        public class GListBoxItem
        {
            public int matchScore;

            private string _myText;
            private string[] _myModifiers;
            private int _myImageIndex;

            public string Label
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    int _length = _myModifiers.Length;
                    if (_length > 0)
                    {
                        sb.Append("[");
                        for (int i = 0; i < _length; i++)
                        {
                            sb.Append(_myModifiers[i]);
                            sb.Append(",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("] ");
                    }
                    sb.Append(_myText);
                    return sb.ToString();
                }
            }

            // properties 
            public string Text
            {
                get { return _myText; }
                set { _myText = value; }
            }
            public string[] Modifiers
            {
                get { return _myModifiers; }
                set { _myModifiers = value; }
            }
            public int ImageIndex
            {
                get { return _myImageIndex; }
                set { _myImageIndex = value; }
            }

            //constructor
            public GListBoxItem(string text, string[] modifiers, int index)
            {
                _myText = text;
                _myModifiers = modifiers;
                _myImageIndex = index;
            }
            public GListBoxItem(string text, string[] modifiers) : this(text, modifiers, -1) { }
            public GListBoxItem() : this("", new string[0]) { }
            public override string ToString()
            {
                return _myText;
            }


        }//End of GListBoxItem class
    }//End of GListBox class
}
