using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTA5Hack
{
    public partial class InputBox : Form
    {
        private string inputText { get; set; }

        public InputBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            inputText = textBox1.Text;
        }

        public T GetValue<T>()
        {
            try
            {
                if (string.IsNullOrEmpty(inputText))
                    throw new Exception();
                if (typeof(T) == typeof(float))
                    return (T)Convert.ChangeType(Convert.ToSingle(inputText), typeof(T));
                throw new NotImplementedException();
            }
            catch
            {
                return default(T);
            }
        }

        public void SetValue(string value)
        {
            textBox1.Text = value;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1.PerformClick();
        }
    }
}
