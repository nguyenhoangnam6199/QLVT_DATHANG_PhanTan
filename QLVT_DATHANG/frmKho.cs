﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT_DATHANG
{
    public partial class frmKho : Form
    {
        public frmKho()
        {
            InitializeComponent();
        }

        private void khoBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            

        }

        private void frmKho_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet.Kho' table. You can move, or remove it, as needed.
           

        }
    }
}
