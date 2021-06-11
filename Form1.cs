using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTA5Hack
{
    public partial class Form1 : Form
    {
        static MemoryHelper GTA5;
        static IntPtr PlayerPTR;
        static int pHealth = 0x280;
        static int pArmor = 0x14E0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            GTA5 = new MemoryHelper("GTA5");

            IntPtr PlayerPTRHook;
            GTA5.aobScanModule(out PlayerPTRHook, "GTA5.exe", "48 8B 05 * * * * 48 85 C0 74 0D 48 39 05");
            PlayerPTR = PlayerPTRHook + GTA5.GetValue<int>(PlayerPTRHook + 3) + 7;
        }

        private void timer_PlayerPTR_Tick(object sender, EventArgs e)
        {
            label2.Text = GTA5.GetValue<IntPtr>(PlayerPTR).ToString("X16");
        }

        private void timer_pHealth_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                GTA5.SetValue(GTA5.GetValue<IntPtr>(PlayerPTR) + pHealth, float.Parse(label4.Text));
            else
                label4.Text = GTA5.GetValue<float>(GTA5.GetValue<IntPtr>(PlayerPTR) + pHealth).ToString();
        }

        private void timer_pArmor_Tick(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                GTA5.SetValue(GTA5.GetValue<IntPtr>(PlayerPTR) + pArmor, float.Parse(label6.Text));
            else
                label6.Text = GTA5.GetValue<float>(GTA5.GetValue<IntPtr>(PlayerPTR) + pArmor).ToString();
        }

        private void timer_pAmmo_Tick(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                GTA5.SetValue<byte>(
                    GTA5.GetValue<IntPtr>(
                        GTA5.GetValue<IntPtr>(PlayerPTR) + 0x10D0
                        ) + 0x78,
                    2);
            else
                GTA5.SetValue<byte>(
                    GTA5.GetValue<IntPtr>(
                        GTA5.GetValue<IntPtr>(PlayerPTR) + 0x10D0
                        ) + 0x78,
                    0);
        }

        private void value_DoubleClick(object sender, EventArgs e)
        {
            InputBox inputBox = new InputBox();
            inputBox.SetValue(((Label)sender).Text);
            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                if (sender == label4)
                    GTA5.SetValue(GTA5.GetValue<IntPtr>(PlayerPTR) + pHealth, inputBox.GetValue<float>());
                else if (sender == label6)
                    GTA5.SetValue(GTA5.GetValue<IntPtr>(PlayerPTR) + pArmor, inputBox.GetValue<float>());
            }
        }
    }
}
