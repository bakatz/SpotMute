using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpotMute.Model;

namespace SpotMute.View
{
    /*
     * Simple class showing the contents of the blacklist.
     */
    public partial class BlackListForm : Form
    {
        private BlackList blist;
        public BlackListForm(BlackList blist)
        {
            this.blist = blist;
            InitializeComponent();
        }

        /*
         * Convert our blacklist to a list. DataGridView only supports lists and arrays. Listener for DataGridView will automatically fire and display the list.
         */
        private void BlackListForm_Load(object sender, EventArgs e)
        {
            blacklistTable.DataSource = blist.getDictionary().ToList();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           //TODO: delete option here.
        }
    }
}
