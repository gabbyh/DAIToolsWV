﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;
using DAILibWV;
using DAILibWV.Frostbite;

namespace DAIToolsWV.ContentTools
{
    public partial class EBXTool : Form
    {
        public byte[] ebxdata = null;
        public EBXFile ebx = null;

        public EBXTool()
        {
            InitializeComponent();
        }

        public void LoadEbx(byte[] data)
        {
            ebxdata = data;
            ebx = new EBXFile(new MemoryStream(ebxdata));
            RefreshDisplay();
        }

        private void loadFromBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "*.bin|*.bin";
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadEbx(File.ReadAllBytes(d.FileName));
        }

        public void RefreshDisplay()
        {
            hb1.ByteProvider = new DynamicByteProvider(ebxdata);
            hb2.ByteProvider = new DynamicByteProvider(ebx.keywordarea);
            rtb1.Text = ebx.HeaderToString();
            listBox1.Items.Clear();
            int count = 0;
            foreach (EBXFile.StreamingPartitionImportEntry exguid in ebx.externalGUIDs)
                listBox1.Items.Add((count++).ToString("X4") + " : " + Helpers.ByteArrayToHexString(exguid.partitionGuid.data4) + " - " + Helpers.ByteArrayToHexString(exguid.instanceGuid.data4));
            count = 0;
            listBox2.Items.Clear();
            foreach (EBXFile.KeyWordDicStruct entry in ebx.keyWordDic)
                listBox2.Items.Add((count++).ToString("X4") + " : Offset = 0x" + entry.offset.ToString("X8") + " Hash = 0x" + entry.hash.ToString("X8") + " Keyword = '" + entry.keyword + "'");
            count = 0;
            listBox3.Items.Clear();
            foreach (EBXFile.StreamingPartitionFieldDescriptor f in ebx.fieldDescriptors)
                listBox3.Items.Add(
                    (count++).ToString("X4")
                    + " : Hash = 0x"
                    + f.fieldNameHash.ToString("X8")
                    + " Type = 0x"
                    + f.flagBits.ToString("X4")
                    + " Reference = 0x"
                    + f.fieldTypeIndex.ToString("X4")
                    + " Offset = 0x"
                    + f.fieldOffset.ToString("X8")
                    + " Secondary Offset = 0x"
                    + f.secondaryOffset.ToString("X8")
                    + " Name ='"
                    + f._name + "'");
            count = 0;
            listBox4.Items.Clear();
            foreach (EBXFile.StreamingPartitionTypeDescriptor f in ebx.complexTypeDescriptors)
                listBox4.Items.Add(
                    (count++).ToString("X4")
                    + " : Hash = 0x"
                    + f.typeNameHash.ToString("X8")
                    + " StartIndex = 0x"
                    + f.layoutDescriptorIndex.ToString("X8")
                    + " NumField = 0x"
                    + f.fieldCount.ToString("X2")
                    + " Alignment = 0x"
                    + f.alignment.ToString("X2")
                    + " Type = 0x"
                    + f.typeFlags.ToString("X4")
                    + " Size = 0x"
                    + f.instanceSize.ToString("X4")
                    + " Secondary Size = 0x"
                    + f.secondaryInstanceSize.ToString("X8")
                    + " Name ='"
                    + f._name + "'");
            count = 0;
            listBox5.Items.Clear();
            foreach (EBXFile.StreamingPartitionInstanceEntry i in ebx.instanceRepeaterList)
                listBox5.Items.Add((count++).ToString("X4") + " : Complex Index = 0x" + i.typeDescriptorIndex.ToString("X4") + " Repeats = 0x" + i.repetitions.ToString("X4"));
            count = 0;
            listBox6.Items.Clear();
            foreach (EBXFile.StreamingPartitionArrayEntry a in ebx.arrayRepeaterList)
                listBox6.Items.Add((count++).ToString("X4") + " : Offset = 0x" +a.offset.ToString("X8") + " Complex Index = 0x" + a.typeDescriptorIndex.ToString("X8") + " Repeats = 0x" + a.elementCount.ToString("X8"));
            treeView1.Nodes.Clear();
            count = 0;
            TreeNode t = new TreeNode("Instances");
            foreach (EBXFile.InstanceStruct ins in ebx.instancesList)
                t = ebx.InstanceTotree(t, ins, count++);
            t.ExpandAll();
            treeView1.Nodes.Add(t);
            rtb2.Text = ebx.toXML();
        }
    }
}
