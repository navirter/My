using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My.Forms
{
    public partial class Loading_progress : Form
    {
        public Loading_progress(int Maximum_value, string Message)
        {
            InitializeComponent();
            Application.DoEvents();

            maximum_value = Maximum_value;
            this.Text = Message + "." + this.Text;
        }

        int maximum_value;
        int last_procent = 0;
        
        private void Loading_progress_Load(object sender, EventArgs e)
        {
            reshow_progress(0);
            PerformLayout();
            Application.DoEvents();
        }        
        public void reshow_progress(int current_value)
        {            
            try
            {
                Application.DoEvents();
                current_value += 1;
                label2.Text = current_value + " из " + maximum_value;
                int procent = current_value * 100 / maximum_value;
                if (procent == last_procent)
                    return;
                progressBar1.Value = procent;
                last_procent = procent;
                PerformLayout();
                Application.DoEvents();
            }
            catch { }
        }
    }
}
