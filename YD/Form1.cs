using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Dictionary<string, string> keyTxtDic = new Dictionary<string, string>();
        private void 导入文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                keyTxtDic.Clear();
                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "文本文件(*.txt)|*.txt";
                file.ShowDialog();
                string url = file.FileName;
                string baseTxt = "";
                if (url != "")
                {
                    using (StreamReader sr = new StreamReader(url, Encoding.UTF8))
                    {
                        baseTxt = sr.ReadToEnd();
                    }
                    List<string> allTitle = new List<string>();
                    List<int> keyNum = new List<int>();
                    Regex reg = new Regex("第(.*?)章");
                    MatchCollection mcs = reg.Matches(baseTxt);
                    if (mcs.Count > 0)
                    {
                        string[] list = reg.Split(baseTxt);
                        List<string> newArray = list.ToList();
                        newArray.RemoveAt(0);
                        foreach (Match mc in mcs)
                        {
                            string t1 = mc.Groups[0].Value;
                            string title = mc.Groups[1].Value;
                            allTitle.Add(title);
                        }
                        for (int i = 0; i < mcs.Count; i++)
                        {
                            string key = (i + 1) + "-" + mcs[i].Groups[0].Value;
                            keyTxtDic.Add(key, newArray[i * 2 + 1]);
                            listBox1.Items.Add(key);
                        }
                        if (File.Exists(@"C:\Logs\TxtLogs.log"))
                        {
                            string ReadMark = File.ReadAllText(@"C:\Logs\TxtLogs.log");
                            if (!string.IsNullOrEmpty(ReadMark))
                            {
                                textBox.Text = keyTxtDic[ReadMark];
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("请上传正确格式文本！");
                    }
                }
            }
            catch (Exception ex)
            {
                SaveErrLog(ex.Message);
            }

        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                书签ToolStripMenuItem.Enabled = true;
                int index = listBox1.IndexFromPoint(e.X, e.Y);
                listBox1.SelectedIndex = index;
                if (listBox1.SelectedIndex != -1)
                {
                    textBox.Text = keyTxtDic[listBox1.SelectedItem.ToString()];
                }
            }
            catch (Exception ex)
            {

                SaveErrLog(ex.Message);
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar.ToString().ToUpper() == Keys.Z.ToString())
                {
                    Hide();
                }
                else if (e.KeyChar.ToString() == "\u001b")
                {
                    Save();
                    Close();
                }
            }
            catch (Exception ex)
            {

                SaveErrLog(ex.Message);
            }
        }

        private void 书签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
                SaveErrLog(ex.Message);
            }
        }
        private void Save()
        {
            string SaveMark = listBox1.SelectedItem.ToString();
            File.WriteAllText(@"C:\Logs\TxtLogs.log", SaveMark, Encoding.UTF8);
        }
        private void SaveErrLog(string log)
        {
            FileStream file = File.OpenWrite(@"C:\Logs\YDErrLog.log");
            byte[] data = Encoding.Default.GetBytes(log + "\r\n");
            file.Write(data, 0, data.Length);
            file.Flush();
            file.Close();
        }
    }
}
