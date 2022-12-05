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
 * Revised: 2022-11-21 - Update to allow for some inventory items (main PC).
 *          2022-12-05 - Added additional companion names to drop down.
 * 
 */
namespace CS_Editor
{
    public partial class CharSelDlg : Form
    {
        #region Properties
        private string _charName = "";
        public string CharName { get { return _charName; } }

        private string _pcName = "";
        public string PCName { set { _pcName = value; } }

        private int _charSelIdx = 0;
        public int CharSelIdx { get { return _charSelIdx; } set { _charSelIdx = value; } }
        #endregion

        // --------------------------------------------------------------------

        #region Constructor
        public CharSelDlg()
        {
            InitializeComponent();
        }
        #endregion

        // --------------------------------------------------------------------

        #region Event Handlers
        private void CharSelDlg_Load(object sender, EventArgs e)
        {
            if (_pcName != "")
            {
                cbNames.Items[0] = _pcName;
            }

            cbNames.SelectedIndex = _charSelIdx;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            _charName = cbNames.Text.Trim();
            _charSelIdx = cbNames.SelectedIndex;

            if (string.IsNullOrEmpty(_charName))
            {
                MessageBox.Show("Need to select a name.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
        #endregion
    }
}
