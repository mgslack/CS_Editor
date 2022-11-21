/*
 * Partical class defining the main character name input dialog for the Colony Ship
 * character editor application.  Case is important when entering main character
 * name.
 * 
 * ----------------------------------------------------------------------------
 * 
 * Author: Michael G. Slack
 * Date Written: 2022-11-21
 * 
 * ----------------------------------------------------------------------------
 * 
 * Revised: yyyy-mm-dd - xxxx.
 * 
 */
namespace CS_Editor
{
    public partial class PCNameDlg : Form
    {
        #region Properties
        private string _charName = "";
        public string CharName { get { return _charName; } set { _charName = value; } }
        #endregion

        // --------------------------------------------------------------------

        #region Constructor
        public PCNameDlg()
        {
            InitializeComponent();
        }
        #endregion

        // --------------------------------------------------------------------

        #region Event Handlers
        private void PCNameDlg_Load(object sender, EventArgs e)
        {
            if (_charName != "")
            {
                tbName.Text = _charName;
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            _charName = tbName.Text.Trim();
            if (string.IsNullOrEmpty(_charName))
            {
                MessageBox.Show("Need to enter a name.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
        #endregion
    }
}
