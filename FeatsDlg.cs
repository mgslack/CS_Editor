using System.Text.RegularExpressions;

/*
 * Partial class defining the feat editing dialog for the Colony Ship character editor
 * application.  Shows the feats the character has and allows adding/removing them.  All
 * known feats (see FeatInfo) are listed by category with their requirement and effect,
 * feats the character already has are grayed out.  Any other feat can be added by typing
 * in its asset name (F_xxx) or full object path.
 *
 * ----------------------------------------------------------------------------
 *
 * Author: Mads Petersen
 * Date Written: 2026-10-27
 *
 * ----------------------------------------------------------------------------
 */
namespace CS_Editor
{
    public partial class FeatsDlg : Form
    {
        #region Properties
        private List<string> _feats = new List<string>();
        public List<string> Feats { get { return _feats; } set { _feats = new List<string>(value); } }

        private bool _featsChanged = false;
        public bool FeatsChanged { get { return _featsChanged; } }
        #endregion

        // --------------------------------------------------------------------

        #region Constructor
        public FeatsDlg()
        {
            InitializeComponent();
        }
        #endregion

        // --------------------------------------------------------------------

        #region Private Methods
        // name for a feat not in the known list, made from asset name (F_C_Loyalty_Jed -> C Loyalty Jed)
        private static string UnknownName(string assetName)
        {
            return Regex.Replace(assetName.Substring(2).Replace('_', ' '), "(?<=[a-z])(?=[A-Z])", " ");
        }

        private void LoadFeatList()
        {
            lvFeats.BeginUpdate();
            lvFeats.Items.Clear();
            foreach (string feat in _feats)
            {
                string asset = SaveFeats.FeatAssetName(feat);
                FeatInfo? info = FeatInfo.Find(asset);
                ListViewItem item = new ListViewItem(info != null ? info.Name : UnknownName(asset));
                item.SubItems.Add(info != null ? info.Category : "Unknown");
                item.SubItems.Add(info != null ? info.Effect : "");
                if (info != null) item.ToolTipText = info.Effect;
                item.Tag = (info != null) ? info : asset;
                lvFeats.Items.Add(item);
            }
            lvFeats.EndUpdate();

            // gray out the feats the character already has
            foreach (ListViewItem item in lvAvailable.Items)
            {
                bool has = _feats.Contains(SaveFeats.FeatPath(((FeatInfo)item.Tag).Asset));
                item.ForeColor = has ? SystemColors.GrayText : SystemColors.WindowText;
            }
        }

        private void LoadAvailableList()
        {
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            lvAvailable.BeginUpdate();
            foreach (FeatInfo info in FeatInfo.KnownFeats)
            {
                if (!groups.ContainsKey(info.Category))
                {
                    groups[info.Category] = new ListViewGroup(info.Category, HorizontalAlignment.Left);
                    lvAvailable.Groups.Add(groups[info.Category]);
                }
                ListViewItem item = new ListViewItem(info.Name, groups[info.Category]);
                item.SubItems.Add(info.Requirement);
                item.SubItems.Add(info.Effect);
                item.ToolTipText = info.Effect;
                item.Tag = info;
                lvAvailable.Items.Add(item);
            }
            lvAvailable.EndUpdate();
        }

        private void ShowDetails(object? tag)
        {
            if (tag is FeatInfo info)
            {
                tbDetails.Text = info.Name + "  (" + info.Category + ", " + info.Asset + ")" + Environment.NewLine +
                                 "Requirement: " + (info.Requirement != "" ? info.Requirement : "none") + Environment.NewLine +
                                 info.Effect;
            }
            else if (tag is string asset)
            {
                tbDetails.Text = UnknownName(asset) + "  (" + asset + ")" + Environment.NewLine +
                                 "Not a known feat, no description available.";
            }
        }

        private void AddFeat(string feat)
        {
            if (_feats.Contains(feat))
            {
                MessageBox.Show("Character already has that feat.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                _feats.Add(feat);
                _featsChanged = true;
                LoadFeatList();
            }
        }
        #endregion

        // --------------------------------------------------------------------

        #region Event Handlers
        private void FeatsDlg_Load(object sender, EventArgs e)
        {
            LoadAvailableList();
            LoadFeatList();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (lvAvailable.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a feat in the list of all feats.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                AddFeat(SaveFeats.FeatPath(((FeatInfo)lvAvailable.SelectedItems[0].Tag).Asset));
            }
        }

        private void BtnAddName_Click(object sender, EventArgs e)
        {
            string text = tbAssetName.Text.Trim();

            if (text.StartsWith("/Game/"))
            {
                AddFeat(text);
            }
            else if (Regex.IsMatch(text, "^F_[A-Za-z0-9_]+$"))
            {
                AddFeat(SaveFeats.FeatPath(text));
            }
            else
            {
                MessageBox.Show("Type in a feat asset name (F_xxx) or full object path.", this.Text, MessageBoxButtons.OK);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lvFeats.SelectedIndices.Count > 0)
            {
                int idx = lvFeats.SelectedIndices[0];
                _feats.RemoveAt(idx);
                _featsChanged = true;
                LoadFeatList();
                if (lvFeats.Items.Count > 0) lvFeats.Items[Math.Min(idx, lvFeats.Items.Count - 1)].Selected = true;
            }
        }

        private void LvFeats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvFeats.SelectedItems.Count > 0) ShowDetails(lvFeats.SelectedItems[0].Tag);
        }

        private void LvAvailable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAvailable.SelectedItems.Count > 0) ShowDetails(lvAvailable.SelectedItems[0].Tag);
        }

        private void LvAvailable_DoubleClick(object sender, EventArgs e)
        {
            BtnAdd_Click(sender, e);
        }
        #endregion
    }
}
