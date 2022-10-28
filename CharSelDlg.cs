/*
 * Partical class defining the character selection dialog for the Colony Ship character editor
 * application.  Fairly simple, has list of NPCs pre-loaded in the combobox drop down.
 * 
 * ----------------------------------------------------------------------------
 * 
 * Author: Michael G. Slack
 * Date Written: 2022-10-27
 * 
 * ----------------------------------------------------------------------------
 * 
 * Revised: yyyy-mm-dd - xxxx.
 * 
 */
namespace CS_Editor
{
    public partial class CharSelDlg : Form
    {
        #region Properties
        private string _charName = "";
        public string CharName { get { return _charName; } set { _charName = value; } }
        #endregion

        #region Constructor
        public CharSelDlg()
        {
            InitializeComponent();
        }
        #endregion

        #region Button Handler
        private void OkBtn_Click(object sender, EventArgs e)
        {
            _charName = cbCharName.Text.Trim();

            if (string.IsNullOrEmpty(_charName))
            {
                MessageBox.Show("Need to enter or select a name.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
        #endregion
    }
}
