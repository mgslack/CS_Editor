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
 * ----------------------------------------------------------------------------
 * 
 * Author: Michael G.Slack
 * Written: 2022-09-19
 *
 * ----------------------------------------------------------------------------
 * 
 * Revised: yyyy-mm-dd - xxxx.
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

        private const string REG_NAME = @"HKEY_CURRENT_USER\Software\Slack and Associates\Tools\CS_Editor";
        private const string REG_KEY1 = "PosX";
        private const string REG_KEY2 = "PosY";
        #endregion

        #region Private Index Maps
        private readonly int[] attrMap = { 6, 11, 16, 21, 26, 31 };
        // skill positions 61 and 65 are unused as of v0.8.247
        private readonly int[] skillMap = { 37, 41, 45, 49, 53, 57, 69, 73, 77, 81, 85, 89, 93, 97, 101, 105, 109, 113 };
        #endregion

        #region Private Variables
        private string gameSaveFn = NOT_LOADED;
        private int charOffset = NOT_SET;
        private NumericUpDown[] skills = new NumericUpDown[MAX_SKILLS];
        private NumericUpDown[] skillsExp = new NumericUpDown[MAX_SKILLS];
        private CheckBox[] skillTags = new CheckBox[MAX_SKILLS];
        private NumericUpDown[] attributes = new NumericUpDown[MAX_ATTRIBUTES];
        private byte[] fileBuffer = new byte[0];
        private int[] charData = new int[MAX_CHAR_INTS];
        private bool charChanged = false;
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
            }
            catch (Exception ex) { /* ignore, go with defaults */ }

            if ((winX != -1) && (winY != -1)) this.SetDesktopLocation(winX, winY);
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
        }

        private byte[] CreateCheckValue()
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

        // As found: https://stackoverflow.com/questions/283456/byte-array-pattern-search/283815#283815
        // Author: Ing. Gerardo Sánchez, answered Jul 28, 2016 at 1:29
        private int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
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
        }

        private void LoadControls()
        {
            nudCurHP.Value = charData[IDX_CHAR_CUR_HP];
            nudExp.Value = charData[IDX_CHAR_CUR_EXP];

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

            charChanged = false;
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
                }
            }
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
        }

        private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Registry.SetValue(REG_NAME, REG_KEY1, this.Location.X);
                Registry.SetValue(REG_NAME, REG_KEY2, this.Location.Y);
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                gameSaveFn = openFileDlg.FileName;
                try
                {
                    fileBuffer = File.ReadAllBytes(gameSaveFn);
                    lblSaveFile.Text = Path.GetFileName(gameSaveFn);
                }
                catch (Exception ex)
                {
                    gameSaveFn = NOT_LOADED; lblSaveFile.Text = NOT_LOADED;
                    MessageBox.Show("Open File - " + ex.Message, this.Text, MessageBoxButtons.OK);
                }
                lblCharName.Text = NOT_LOADED; charOffset = NOT_SET;
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
                selDlg.CharName = lblCharName.Text;
                if (selDlg.ShowDialog() == DialogResult.OK)
                {
                    lblCharName.Text = selDlg.CharName;
                    byte[] nameVal = CreateCheckValue();
                    charOffset = Search(fileBuffer, nameVal);
                    if (charOffset == NOT_SET)
                    {
                        MessageBox.Show(lblCharName.Text + " not found in save game file.", this.Text,
                            MessageBoxButtons.OK);
                        lblCharName.Text = NOT_LOADED; charChanged = false;
                    }
                    else
                    {
                        charOffset += nameVal.Length + 1;
                        LoadCharData();
                        lblCharName.Text += "  (lvl: " + charData[IDX_CHAR_LVL] + ")";
                        LoadControls();
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
                Process.Start(htmlPath);
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
        #endregion
    }
}