using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace exprodtoolbox
{
    public partial class frmSelectColums : Form
    {
        public List<FieldInfo> Fields = FieldInfo.ReadFromRes();
        public List<FieldInfo> src;
        public List<FieldInfo> dest;
        private int indexOfItemUnderMouseToDrop;

        public frmSelectColums()
        {
            InitializeComponent();
            lbSource.Items.AddRange(Fields.ToArray());
            //if (Properties.Settings.Default.FieldsLists!=null)
            foreach (string s in Properties.Settings.Default.FieldsLists)
            {
                cbSETS.Items.Add(s);
            }
        }
        /*
        private void ListDragSource_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ListBox ListDragSource = (sender as ListBox);
            if (ListDragSource.SelectedItems.Count == 0) return;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {

                string ddrop = null;
                foreach (object o in ListDragSource.SelectedItems)
                {
                    ddrop += (o as FieldInfo).DragDrop + "\n\r";
                }
                // Proceed with the drag-and-drop, passing in the list item.                    
                DragDropEffects dropEffect = ListDragSource.DoDragDrop(ddrop, DragDropEffects.All);

                // If the drag operation was a move then remove the item.
                if (dropEffect == DragDropEffects.Move)
                {
                    ListDragSource.BeginUpdate();
                    object[] twodel = new object[ListDragSource.SelectedItems.Count];
                    ListDragSource.SelectedItems.CopyTo(twodel, 0);
                    foreach (object o in twodel)
                    {
                        ListDragSource.Items.Remove(o);
                    }
                    ListDragSource.EndUpdate();
                }

            }
        }
        */
        private void DD_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ListBox ListDragSource = (sender as ListBox);
            //if (ListDragSource.SelectedItems.Count == 0) return;
            int index = ListDragSource.IndexFromPoint(e.X, e.Y);
            string s = ((ListDragSource.Items[index]) as FieldInfo).DragDrop;
            DragDropEffects dde1 = DoDragDrop(s,
                DragDropEffects.Move);

            if (dde1 == DragDropEffects.Move)
            {
                ListDragSource.Items.RemoveAt(index);
            }
        }

        private void ListDragTarget_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            ListBox ListDragTarget = (sender as ListBox);

            // Ensure that the list item index is contained in the data.
            if (e.Data.GetDataPresent(DataFormats.Text))
            {

                string it = (string)e.Data.GetData(typeof(string));
                List<FieldInfo> itemz = FieldInfo.ReadFromString(it);

                // Perform drag-and-drop, depending upon the effect.
                if (e.Effect == DragDropEffects.Move)
                {

                    // Insert the item.
                    if (indexOfItemUnderMouseToDrop != ListBox.NoMatches)
                        foreach (FieldInfo item in itemz)
                        {
                            ListDragTarget.Items.Insert(indexOfItemUnderMouseToDrop, item);

                        }
                    else

                        foreach (FieldInfo item in itemz)
                        {
                            ListDragTarget.Items.Add(item);
                        }


                }
            }
        }

        private void ListDragTarget_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            ListBox ListDragTarget = (sender as ListBox);

            // By default, the drop action should be move, if allowed.
            e.Effect = DragDropEffects.Move;

            indexOfItemUnderMouseToDrop =
            ListDragTarget.IndexFromPoint(ListDragTarget.PointToClient(new Point(e.X, e.Y)));


        }
        /*
        private void lbSource_DoubleClick(object sender, EventArgs e)
        {
            DClick(lbSource, lbSelect);
        }


        private void lbSelect_DoubleClick(object sender, EventArgs e)
        {
            DClick(lbSelect, lbSource);
        }



        private void DClick(object sender, object reciver)
        {
            ListBox ListDragSource = (sender as ListBox);
            ListBox ListDragTarget = (reciver as ListBox);
            if (ListDragSource.SelectedItems.Count == 0) return;
            string ddrop = null;
            foreach (object o in ListDragSource.SelectedItems)
            {
                ddrop += (o as FieldInfo).DragDrop + "\n\r";
            }

            ListDragSource.BeginUpdate();
            object[] twodel = new object[ListDragSource.SelectedItems.Count];
            ListDragSource.SelectedItems.CopyTo(twodel, 0);
            foreach (object o in twodel)
            {
                ListDragSource.Items.Remove(o);
            }
            ListDragSource.EndUpdate();
            /////////////////////////////////////////////////////////////////
            List<FieldInfo> itemz = FieldInfo.ReadFromString(ddrop);
            foreach (FieldInfo item in itemz)
            {
                ListDragTarget.Items.Add(item);
            }
        }
        */
        private void bOK_Click(object sender, EventArgs e)
        {
            string tmp = null;
            foreach (object o in lbSelect.Items)
            {
                tmp += (o as FieldInfo).DragDrop + "\n\r";
            }

            XmlSerializer s = new XmlSerializer(typeof(List<FieldInfo>));

            switch (cbOP.SelectedIndex)
            {
                case 1:
                    if (string.IsNullOrEmpty(tmp)) break;

                    dest = FieldInfo.ReadFromString(tmp);
                    MemoryStream memStream = new MemoryStream();
                    XmlTextWriter xmlWriter;
                    xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                    xmlWriter.Namespaces = true;
                    s.Serialize(xmlWriter, dest);
                    xmlWriter.Close();
                    memStream.Close();
                    string xml;
                    xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                    xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                    xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                    Properties.Settings.Default.FieldsLists.Add(xml);
                    Properties.Settings.Default.Save();
                    break;//save
                case 2:
                    if (string.IsNullOrEmpty(cbSETS.SelectedItem.ToString())) { this.DialogResult = DialogResult.Cancel; break; }
                    dest = (List<FieldInfo>)s.Deserialize(new XmlTextReader(new StringReader(cbSETS.SelectedItem.ToString()))); break;//load
                case 3:
                    if (string.IsNullOrEmpty(cbSETS.SelectedItem.ToString())) break;
                    this.DialogResult = DialogResult.Cancel;
                    Properties.Settings.Default.FieldsLists.Remove(cbSETS.SelectedItem.ToString());
                    Properties.Settings.Default.Save();

                    break;//del
                default:
                    if (string.IsNullOrEmpty(tmp)) { this.DialogResult = DialogResult.Cancel; break; }
                    dest = FieldInfo.ReadFromString(tmp);
                    break;
            }



        }

        private void lbSource_DoubleClick_1(object sender, EventArgs e)
        {

        }


    }
}
