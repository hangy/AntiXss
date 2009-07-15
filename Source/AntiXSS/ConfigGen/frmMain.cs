using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator
{
    internal partial class frmMain : Form
    {
        bool blnAnalysisComplete = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!blnAnalysisComplete)
                {
                    MessageBox.Show(this, LocalizationUtil.GetString("AnalysisNotCompletedMessage"), LocalizationUtil.GetString("AnalysisNotCompletedTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dlgAssembly.FileNames.Length > 0 && Directory.Exists(Path.GetDirectoryName(dlgAssembly.FileNames[0])))
                {
                    dlgGenConfig.Description = LocalizationUtil.GetString("dlgGenConfigDescription");
                    dlgGenConfig.ShowNewFolderButton = true;
                    dlgGenConfig.SelectedPath = Path.GetDirectoryName(dlgAssembly.FileNames[0]);
                    if (dlgGenConfig.ShowDialog() == DialogResult.OK)
                    {
                        string configFilename = Path.Combine(dlgGenConfig.SelectedPath, "antixssmodule.config");
                        if (File.Exists(configFilename) && MessageBox.Show(this, LocalizationUtil.GetString("FileOverwriteMessage"), LocalizationUtil.GetString("FileOverwriteTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            return;
                        }

                        
                            this.Cursor = Cursors.WaitCursor;
                            List<string> controls = new List<string>();
                            foreach (ListViewItem lvwItem in lvwAnalysis.Items)
                            {
                                if (!controls.Contains(lvwItem.SubItems[2].Text))
                                    controls.Add(lvwItem.SubItems[2].Text);
                            }
                            ConfigGenerator cg = new ConfigGenerator();
                            ConfigOptions co = new ConfigOptions();
                            co.doubleEncodingFilter = chkDoubleEncoding.Checked;
                            co.encodeDerivedControls = chkEncodeDerivedControls.Checked;
                            co.markAntiXssOutput = chkMarkOutput.Checked;
                            co.markAntiXssOutputColor = Color.FromName((string)cboMarkOutputColor.SelectedItem);
                            cg.GenerateConfig(controls, co, configFilename);
                            this.Cursor = Cursors.Default;
                            MessageBox.Show(this, LocalizationUtil.GetString("ConfigGenCompleteMessage"), LocalizationUtil.GetString("ConfigGenCompleteTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        

                    }
                }
            }
            catch
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(this, LocalizationUtil.GetString("ConfigGenUnknownErrorMessage"), LocalizationUtil.GetString("UnknownErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.LocalizeControls();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            dlgAssembly.Title = LocalizationUtil.GetString("dlgAssemblyTitle");
            dlgAssembly.AddExtension = true;
            dlgAssembly.CheckFileExists = true;
            dlgAssembly.CheckPathExists = true;
            dlgAssembly.DefaultExt = ".dll";
            dlgAssembly.DereferenceLinks = true;
            dlgAssembly.Multiselect = true;
            dlgAssembly.ValidateNames = true;
            dlgAssembly.Filter = "ASP.NET Assembly (*.dll)|*.dll";
            if (dlgAssembly.ShowDialog()==DialogResult.OK) 
            {
                txtAssemblyFile.Text = string.Join(",",dlgAssembly.FileNames);
                blnAnalysisComplete = false;
            }

        }

        private void LocalizeControls() 
        {
            this.Text = LocalizationUtil.GetString(this.Name);
            foreach (Control c in this.Controls) 
            {
                c.Text = LocalizationUtil.GetString(c.Name);
                hlpControls.SetHelpString(c,LocalizationUtil.GetHelpString(c.Name));
            }
            foreach (Control c in this.grpOptions.Controls)
            {
                c.Text = LocalizationUtil.GetString(c.Name);
                hlpControls.SetHelpString(c,LocalizationUtil.GetHelpString(c.Name));
            }

            hlpControls.SetHelpString(lvwAnalysis,LocalizationUtil.GetHelpString(lvwAnalysis.Name));

            foreach (ColumnHeader ch in lvwAnalysis.Columns) 
            {
                ch.Text = LocalizationUtil.GetString(ch.Text);
            }
            cboMarkOutputColor.Items.Clear();
            foreach (string color in Enum.GetNames(typeof(KnownColor))) 
            {
                cboMarkOutputColor.Items.Add(color);
            }
            cboMarkOutputColor.SelectedItem = "Yellow";
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAssemblyFile.Text))
            {
                MessageBox.Show(this, LocalizationUtil.GetString("NoFileSelectedErrorMessage"), LocalizationUtil.GetString("NoFileSelectedErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            try
            {
                lvwAnalysis.Items.Clear();
                AssemblyAnalysis aa = new AssemblyAnalysis();
                aa.ControlFound += new EventHandler<AssemblyAnalysis.AnalysisEventArgs>(aa_ControlFound);
                foreach (string s in txtAssemblyFile.Text.Split(','))
                {
                    aa.Analyze(s);
                }
                this.Cursor = Cursors.Default;
                blnAnalysisComplete = true;
                MessageBox.Show(this, LocalizationUtil.GetString("AnalysisCompleteMessage"), LocalizationUtil.GetString("AnalysisCompleteTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            catch (FileNotFoundException fnfe)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(this, string.Format(LocalizationUtil.GetString("FileNotFoundExceptionMessage"), fnfe.FileName), LocalizationUtil.GetString("FileNotFoundExceptionTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch 
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(this, LocalizationUtil.GetString("AnalysisUnknownErrorMessage"), LocalizationUtil.GetString("UnknownErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            
        }

        void aa_ControlFound(object sender, AssemblyAnalysis.AnalysisEventArgs e)
        {
            ListViewItem lvwItem = new ListViewItem(e.ControlDetail.pageName);
            lvwItem.SubItems.Add(e.ControlDetail.controlName);
            lvwItem.SubItems.Add(e.ControlDetail.controlType);
            lvwAnalysis.Items.Add(lvwItem);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
