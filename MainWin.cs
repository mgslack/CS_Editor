using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;

/*
 * Program used to edit Colony Ship save game files to edit party members.
 * 
 * Notes:
 * Skill exp is as follows (exp till next level, resets to zero):
 *  1 : 20
 *  2 : 100
 *  3 : 300
 *  4 : 600
 *  5 : 1200
 *  6 : 2400
 *  7 : 4800
 *  8 : 9600
 *  9 : (don't think you can get here naturally, w/o tagging)
 *  
 * Tagging a skill adds two levels to base, but lvl/exp gain like 2 levels below
 * (lvl 2, tagged, shows lvl 4, but only need 100 exp for lvl 5)
 * 
 * The skill list in the save game file has the values for Critical Strike and
 * Evasion swapped versus what the game UI shows.  Editor keeps same order as
 * game UI.
 * 
 * Two of the skill slots in the save file for a character are not used, or at
 * least not displayed.  All characters had the two slots (61 and 65) set to
 * '1', no exp, not tagged.  This may change with later versions of the game.
 * 
 * Added functionality to allow editing of selected inventory amounts, such as
 * credits, medkits and ammo.  For any of them, it is possibly play has none
 * available so counts would not be editable for those not available.  Inventory
 * counts only editable when updating main (created) PC.
 * 
 * Note: will prompt for and load main (created) PC when opening a save file.
 * If not found, file will not be 'opened'.
 * 
 * ----------------------------------------------------------------------------
 * 
 * Author: Michael G.Slack
 * Written: 2022-09-19
 *
 * ----------------------------------------------------------------------------
 * 
 * Revised: 2022-11-21 - Added editing a few inventory amounts (credits, medkits,
 *                       and different ammos).
 *          2022-11-22 - Added about box dialog, updated some 'magic numbers' to
 *                       be constants.
 * 
 */
namespace CS_Editor
{
    public partial class MainWin : Form
    {
        #region Private Consts
        private const string HTML_HELP_FILE = "CS_Editor_help.html";
        private const string NOT_LOADED = "<none>";
        private const int NOT_SET = -1;
        private const int MAX_SKILLS = 18;
        private const int MAX_ATTRIBUTES = 6;
        private const int MAX_CHAR_INTS = 121;
        private const int IDX_CHAR_CUR_HP = 2;
        private const int IDX_CHAR_LVL = 116;
        private const int IDX_CHAR_CUR_EXP = 117;
        private const int MAX_INV_COUNTS = 7;
        private const int INV_COUNT_OFFSET = 72;

        // inventory counts can edit (location strings)
        private const string CREDITS_LOC_STR = "III_Credits.III_Credits_C"; // +72 bytes = credits value
        private const string MEDKITS_LOC_STR = "III_Meds.III_Meds_C"; // +72 bytes = medkits value
        private const string AMMO_9MM_LOC_STR = "III_Ammo_9mm.III_Ammo_9mm_C"; // +72 bytes = 9mm count value
        private const string AMMO_45_LOC_STR = "III_Ammo_45.III_Ammo_45_C"; // +72 bytes = .45 count value
        private const string AMMO_556_LOC_STR = "III_Ammo_556.III_Ammo_556_C"; // +72 bytes = 5.56 count value
        private const string AMMO_SHELL_LOC_STR = "III_Ammo_Shells.III_Ammo_Shells_C"; // +72 bytes = shells count value
        private const string AMMO_CELL_LOC_STR = "III_Ammo_Cell.III_Ammo_Cell_C"; // +72 bytes = cell count value
        private const string END_OF_INV_STR = "Items Grid Position"; // end of inventory marker (main PC)

        // registry key strings
        private const string REG_NAME = @"HKEY_CURRENT_USER\Software\Slack and Associates\Tools\CS_Editor";
        private const string REG_KEY1 = "PosX";
        private const string REG_KEY2 = "PosY";
        private const string REG_KEY3 = "LastMainPCName";
        #endregion

        #region Private Index Maps
        private readonly int[] attrMap = { 6, 11, 16, 21, 26, 31 };
        // skill positions 61 and 65 are unused as of v0.8.247
        private readonly int[] skillMap = { 37, 41, 45, 49, 53, 57, 69, 73, 77, 81, 85, 89, 93, 97, 101, 105, 109, 113 };
        #endregion

        #region Private Variables
        private string gameSaveFn = NOT_LOADED;
        private int charOffset = NOT_SET;
        private string mainPCName = "";
        private NumericUpDown[] skills = new NumericUpDown[MAX_SKILLS];
        private NumericUpDown[] skillsExp = new NumericUpDown[MAX_SKILLS];
        private CheckBox[] skillTags = new CheckBox[MAX_SKILLS];
        private NumericUpDown[] attributes = new NumericUpDown[MAX_ATTRIBUTES];
        private byte[] fileBuffer = new byte[0];
        private int[] charData = new int[MAX_CHAR_INTS];
        private bool charChanged = false;
        private int charSelIdx = 0;
        // order of inventory: credits, medkits, 9mm, .45, 5.56, shells, cells
        private int[] invOffsets = { NOT_SET, NOT_SET, NOT_SET, NOT_SET, NOT_SET, NOT_SET, NOT_SET };
        private int[] invAmounts = { NOT_SET, NOT_SET, NOT_SET, NOT_SET, NOT_SET, NOT_SET, NOT_SET };
        private NumericUpDown[] inventory = new NumericUpDown[MAX_INV_COUNTS];
        #endregion

        // --------------------------------------------------------------------

        #region Private Methods
        private void LoadRegistryValues()
        {
            int winX = -1, winY = -1;
            
            try
            {
                winX = (int)Registry.GetValue(REG_NAME, REG_KEY1, winX);
                winY = (int)Registry.GetValue(REG_NAME, REG_KEY2, winY);
                mainPCName = (string)Registry.GetValue(REG_NAME, REG_KEY3, "");
                if (mainPCName == null) { mainPCName = ""; }
            }
            catch (Exception ex) { /* ignore, go with defaults */ }

            if ((winX != -1) && (winY != -1)) this.SetDesktopLocation(winX, winY);
        }

        private void WriteRegistryValues()
        {
            // only one to save, only write if not blank
            if (mainPCName != "") { Registry.SetValue(REG_NAME, REG_KEY3, mainPCName); }
        }

        private void SetupContextMenu()
        {
            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuAbout = new ToolStripMenuItem("About");

            mnuAbout.Click += new EventHandler(mnuAbout_Click);
            mnu.Items.AddRange(new ToolStripMenuItem[] { mnuAbout });
            this.ContextMenuStrip = mnu;
        }

        private void InitControls()
        {
            // setup skill arrays
            skills[0] = nudBladed;
            skillsExp[0] = nudBladedExp;
            skillTags[0] = cbBladed;
            skills[1] = nudBlunt;
            skillsExp[1] = nudBluntExp;
            skillTags[1] = cbBlunt;
            skills[2] = nudPistol;
            skillsExp[2] = nudPistolExp;
            skillTags[2] = cbPistol;
            skills[3] = nudShotgun;
            skillsExp[3] = nudShotgunExp;
            skillTags[3] = cbShotgun;
            skills[4] = nudRifle;
            skillsExp[4] = nudRifleExp;
            skillTags[4] = cbRifle;
            skills[5] = nudSMG;
            skillsExp[5] = nudSMGExp;
            skillTags[5] = cbSMG;
            skills[6] = nudEvasion;  // evasion comes before critical strike in save file
            skillsExp[6] = nudEvasionExp; // but is shown after in UI
            skillTags[6] = cbEvasion;
            skills[7] = nudCritical;
            skillsExp[7] = nudCriticalExp;
            skillTags[7] = cbCritical;
            skills[8] = nudArmor;
            skillsExp[8] = nudArmorExp;
            skillTags[8] = cbArmor;
            skills[9] = nudBiotech;
            skillsExp[9] = nudBiotechExp;
            skillTags[9] = cbBioTech;
            skills[10] = nudComputers;
            skillsExp[10] = nudComputersExp;
            skillTags[10] = cbComputers;
            skills[11] = nudElectronics;
            skillsExp[11] = nudElectronicsExp;
            skillTags[11] = cbElectronics;
            skills[12] = nudPersuasion;
            skillsExp[12] = nudPersuasionExp;
            skillTags[12] = cbPersuasion;
            skills[13] = nudStreetwise;
            skillsExp[13] = nudStreetwiseExp;
            skillTags[13] = cbStreetwise;
            skills[14] = nudImpersonate;
            skillsExp[14] = nudImpersonateExp;
            skillTags[14] = cbImpersonate;
            skills[15] = nudLockpick;
            skillsExp[15] = nudLockpickExp;
            skillTags[15] = cbLockpick;
            skills[16] = nudSteal;
            skillsExp[16] = nudStealExp;
            skillTags[16] = cbSteal;
            skills[17] = nudSneak;
            skillsExp[17] = nudSneakExp;
            skillTags[17] = cbSneak;

            // setup attribute array
            attributes[0] = nudStrength;
            attributes[1] = nudDexterity;    // dex comes before con in file
            attributes[2] = nudConstitution; // but are shown con followed by dex
            attributes[3] = nudPerception;
            attributes[4] = nudIntelligence;
            attributes[5] = nudCharisma;

            // setup inventory counts array
            inventory[0] = nudCredits;
            inventory[1] = nudMedKits;
            inventory[2] = nud9mm;
            inventory[3] = nud45;
            inventory[4] = nud556;
            inventory[5] = nudShells;
            inventory[6] = nudCells;
        }

        private byte[] CreateNameCheckValue()
        {
            int nameLength = lblCharName.Text.Length;
            byte[] val = new byte[9 + nameLength];

            // check value '00 04 00 00 00 [name length + 1] 00 00 00 [name]'
            for (int i = 0; i < val.Length; i++)
            {
                val[i] = 0;
            }
            val[1] = 4; val[5] = (byte)(nameLength + 1);
            for (int i = 0; i < nameLength; i++)
            {
                val[9 + i] = (byte)lblCharName.Text[i];
            }

            return val;
        }

        private byte[] CreateStrCheckValue(string lookingFor)
        {
            byte[] val = new byte[lookingFor.Length];

            for (int i = 0; i < val.Length; i++) val[i] = (byte)lookingFor[i];

            return val;
        }

        // As found: https://stackoverflow.com/questions/283456/byte-array-pattern-search/283815#283815
        // Author: Ing. Gerardo Sánchez, answered Jul 28, 2016 at 1:29
        private int Search(byte[] src, byte[] pattern, int startPos)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = startPos; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return NOT_SET;
        }

        private int SearchFor(string lookingFor, int maxOffset)
        {
            byte[] val = CreateStrCheckValue(lookingFor);

            int offset = Search(fileBuffer, val, charOffset);
            if (offset > maxOffset) offset = NOT_SET;

            if (offset != NOT_SET && lookingFor != END_OF_INV_STR)
            {
                offset += lookingFor.Length + INV_COUNT_OFFSET;
            }

            return offset;
        }

        private void LoadInvDataNow()
        {
            using (MemoryStream memFile = new MemoryStream(fileBuffer))
            {
                using (BinaryReader reader = new BinaryReader(memFile))
                {
                    for (int i = 0; i < MAX_INV_COUNTS; i++)
                    {
                        if (invOffsets[i] != NOT_SET)
                        {
                            memFile.Position = invOffsets[i];
                            invAmounts[i] = reader.ReadInt32();
                        }
                        else
                        {
                            invAmounts[i] = NOT_SET;
                        }
                    }
                }
            }
        }

        private void LoadInventoryData()
        {
            if (lblCharName.Text == mainPCName)
            {
                int maxOffset = SearchFor(END_OF_INV_STR, fileBuffer.Length);

                invOffsets[0] = SearchFor(CREDITS_LOC_STR, maxOffset);
                invOffsets[1] = SearchFor(MEDKITS_LOC_STR, maxOffset);
                invOffsets[2] = SearchFor(AMMO_9MM_LOC_STR, maxOffset);
                invOffsets[3] = SearchFor(AMMO_45_LOC_STR, maxOffset);
                invOffsets[4] = SearchFor(AMMO_556_LOC_STR, maxOffset);
                invOffsets[5] = SearchFor(AMMO_SHELL_LOC_STR, maxOffset);
                invOffsets[6] = SearchFor(AMMO_CELL_LOC_STR, maxOffset);
                LoadInvDataNow();
            }
            else
            {
                for (int i = 0; i < MAX_INV_COUNTS; i++)
                {
                    invOffsets[i] = NOT_SET;
                    invAmounts[i] = NOT_SET;
                }
            }
        }

        private void LoadCharData()
        {
            using (MemoryStream memFile = new MemoryStream(fileBuffer))
            {
                memFile.Position = charOffset;

                using (BinaryReader reader = new BinaryReader(memFile))
                {
                    for (int i = 0; i < charData.Length; i++)
                    {
                        charData[i] = reader.ReadInt32();
                    }
                }
            }

            LoadInventoryData();
        }

        private void LoadInventoryControls()
        {
            for (int i = 0; i < MAX_INV_COUNTS; i++)
            {
                inventory[i].Enabled = (invAmounts[i] != NOT_SET);
                if (inventory[i].Enabled)
                {
                    inventory[i].Value = invAmounts[i];
                }
                else
                {
                    inventory[i].Value = 0;
                }
            }
        }

        private void LoadControls()
        {
            nudCurHP.Value = charData[IDX_CHAR_CUR_HP];
            nudExp.Value = charData[IDX_CHAR_CUR_EXP];

            skills[0].Focus();
            for (int i = 0; i < MAX_SKILLS; i++)
            {
                skills[i].Value = charData[skillMap[i]];
                skillsExp[i].Value = charData[skillMap[i]+1];
                skillTags[i].Checked = charData[skillMap[i] + 2] == 1;
            }

            for (int i = 0; i < MAX_ATTRIBUTES; i++)
            {
                attributes[i].Value = charData[attrMap[i]];
            }

            LoadInventoryControls();

            charChanged = false;
        }

        private void UnloadInventoryControls()
        {
            for (int i = 0; i < MAX_INV_COUNTS; i++)
            {
                if (inventory[i].Enabled)
                {
                    invAmounts[i] = (int)inventory[i].Value;
                }
            }
        }

        private void UnLoadControls()
        {
            charData[IDX_CHAR_CUR_HP] = (int)nudCurHP.Value;
            charData[IDX_CHAR_CUR_EXP] = (int)nudExp.Value;

            for (int i = 0; i < MAX_SKILLS; i++)
            {
                charData[skillMap[i]] = (int)skills[i].Value;
                charData[skillMap[i] + 1] = (int)skillsExp[i].Value;
                charData[skillMap[i] + 2] = skillTags[i].Checked ? 1 : 0;
            }

            for (int i = 0; i < MAX_ATTRIBUTES; i++)
            {
                charData[attrMap[i]] = (int)attributes[i].Value;
            }

            UnloadInventoryControls();
        }

        private void SaveInventoryData(MemoryStream memFile, BinaryWriter writer)
        {
            for (int i = 0; i < MAX_INV_COUNTS; i++)
            {
                if (invOffsets[i] != NOT_SET)
                {
                    memFile.Position = invOffsets[i];
                    writer.Write(invAmounts[i]);
                }
            }
        }

        private void SaveCharData()
        {
            using (MemoryStream memFile = new MemoryStream(fileBuffer))
            {
                memFile.Position = charOffset;

                using (BinaryWriter writer = new BinaryWriter(memFile))
                {
                    for (int i = 0; i < charData.Length; i++)
                    {
                        writer.Write(charData[i]);
                    }

                    SaveInventoryData(memFile, writer);
                }
            }
        }

        private bool FindChar(string name)
        {
            lblCharName.Text = name;
            byte[] nameVal = CreateNameCheckValue();
            charOffset = Search(fileBuffer, nameVal, 0);
            if (charOffset == NOT_SET)
            {
                MessageBox.Show(lblCharName.Text + " not found in save game file.", this.Text,
                    MessageBoxButtons.OK);
                lblCharName.Text = NOT_LOADED; charChanged = false;
                return false;
            }
            else
            {
                charOffset += nameVal.Length + 1;
                LoadCharData();
                lblCharName.Text += "  (lvl: " + charData[IDX_CHAR_LVL] + ")";
                LoadControls();
                return true;
            }
        }

        private bool GetMainChar()
        {
            PCNameDlg dlg = new PCNameDlg();
            bool gotPC = false;

            dlg.CharName = mainPCName;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                mainPCName = dlg.CharName;
                gotPC = FindChar(mainPCName);
            }
            dlg.Dispose();

            return gotPC;
        }
        #endregion

        // --------------------------------------------------------------------

        public MainWin()
        {
            InitializeComponent();
        }

        // --------------------------------------------------------------------

        #region Form Event Handlers
        private void MainWin_Load(object sender, EventArgs e)
        {
            LoadRegistryValues();
            InitControls();
            SetupContextMenu();
        }

        private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            WriteRegistryValues();
            if (this.WindowState == FormWindowState.Normal)
            {
                Registry.SetValue(REG_NAME, REG_KEY1, this.Location.X);
                Registry.SetValue(REG_NAME, REG_KEY2, this.Location.Y);
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            bool setBack = false;

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                gameSaveFn = openFileDlg.FileName;
                try
                {
                    fileBuffer = File.ReadAllBytes(gameSaveFn);
                    lblSaveFile.Text = Path.GetFileName(gameSaveFn);
                    setBack = !GetMainChar();
                }
                catch (Exception ex)
                {
                    setBack = true;
                    MessageBox.Show("Open File - " + ex.Message, this.Text, MessageBoxButtons.OK);
                }
                if (setBack)
                {
                    gameSaveFn = NOT_LOADED; lblSaveFile.Text = NOT_LOADED;
                    lblCharName.Text = NOT_LOADED; charOffset = NOT_SET;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (lblCharName.Text != NOT_LOADED && gameSaveFn != NOT_LOADED && charOffset != NOT_SET)
            {
                DialogResult res = DialogResult.Yes;
                if (!charChanged)
                {
                    res = MessageBox.Show("Character data not changed, save anyway?", this.Text,
                        MessageBoxButtons.YesNo);
                }

                if (res == DialogResult.Yes) {
                    UnLoadControls();
                    SaveCharData();
                    File.WriteAllBytes(gameSaveFn, fileBuffer);
                    charChanged = false;
                    MessageBox.Show("Character saved to save game file.", this.Text, MessageBoxButtons.OK);
                }
            }
        }

        private void BtnFind_Click(object sender, EventArgs e)
        {
            DialogResult res = DialogResult.Yes;

            if (charChanged)
            {
                res = MessageBox.Show("Current character has been changed, lose changes?", this.Text,
                    MessageBoxButtons.YesNo);
            }

            if (gameSaveFn != NOT_LOADED && res == DialogResult.Yes)
            {
                CharSelDlg selDlg = new CharSelDlg();
                selDlg.CharSelIdx = charSelIdx;
                selDlg.PCName = mainPCName;
                if (selDlg.ShowDialog() == DialogResult.OK)
                {
                    charSelIdx = selDlg.CharSelIdx;
                    // only search for new name if looking for different name
                    if (lblCharName.Text != selDlg.CharName)
                    {
                        _ = FindChar(selDlg.CharName); // ignore true/false return
                    }
                }
                selDlg.Dispose();
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            var asm = Assembly.GetEntryAssembly();
            var asmLocation = Path.GetDirectoryName(asm.Location);
            var htmlPath = Path.Combine(asmLocation, HTML_HELP_FILE);

            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = true; // needed for .net core????
                p.StartInfo.FileName = htmlPath;
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Cannot load help: " + ex.Message, this.Text, MessageBoxButtons.OK);
            }
        }

        private void Xxx_ValueChanged(object sender, EventArgs e)
        {
            charChanged = true;
        }

        private void Xxx_CheckedChanged(object sender, EventArgs e)
        {
            charChanged = true;
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();

            about.ShowDialog(this);
            about.Dispose();
        }
        #endregion
    }
}