using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UaAsk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            Task<SearchInfo> search = new Task<SearchInfo>(() => Parser.getData(textBox1.Text));
            search.Start();

            while (search.Status == TaskStatus.Running)
            {
                Application.DoEvents();
            }

            dataGridView1.Rows.Clear();
            for(int i = 0; i<search.Result.Count; i++)
            {
                dataGridView1.Rows.Add();
                var cell = dataGridView1.Rows[i].Cells[0];
                dataGridView1.Rows[i].Cells[0].Value = search.Result.headers[i];
                dataGridView1.Rows[i].Cells[1].Value = search.Result.urls[i];
                dataGridView1.Rows[i].Cells[2].Value = search.Result.adv[i];
            }


            button1.Enabled = true;
        }
    }
}
