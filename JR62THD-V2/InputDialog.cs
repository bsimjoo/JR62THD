using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JR62THD_V2 {
    public partial class InputDialog : Form {
        public InputDialog() {
            InitializeComponent();
        }
        public string Value { get; set; }
        public bool ForAll { get; set; } = true;
        private void textBox1_TextChanged(object sender, EventArgs e) {
            Value = textBox1.Text;
        }

        private void InputDialog_Load(object sender, EventArgs e) {
            textBox1.Text = Value;
            checkBox1.Checked = ForAll;
            this.Focus();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            ForAll = checkBox1.Checked;
        }
    }
}
