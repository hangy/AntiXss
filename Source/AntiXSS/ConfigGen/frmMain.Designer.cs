namespace Microsoft.Security.Application.SecurityRuntimeEngine.ConfigurationGenerator
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblMainText = new System.Windows.Forms.Label();
            this.lblProjectDir = new System.Windows.Forms.Label();
            this.txtAssemblyFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.cboMarkOutputColor = new System.Windows.Forms.ComboBox();
            this.chkMarkOutput = new System.Windows.Forms.CheckBox();
            this.chkEncodeDerivedControls = new System.Windows.Forms.CheckBox();
            this.chkDoubleEncoding = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.grpAnalysis = new System.Windows.Forms.GroupBox();
            this.lvwAnalysis = new System.Windows.Forms.ListView();
            this.clmnPage = new System.Windows.Forms.ColumnHeader();
            this.clmnControl = new System.Windows.Forms.ColumnHeader();
            this.clmnControlType = new System.Windows.Forms.ColumnHeader();
            this.dlgAssembly = new System.Windows.Forms.OpenFileDialog();
            this.dlgGenConfig = new System.Windows.Forms.FolderBrowserDialog();
            this.hlpControls = new System.Windows.Forms.HelpProvider();
            this.grpOptions.SuspendLayout();
            this.grpAnalysis.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMainText
            // 
            this.lblMainText.Location = new System.Drawing.Point(12, 9);
            this.lblMainText.Name = "lblMainText";
            this.lblMainText.Size = new System.Drawing.Size(571, 57);
            this.lblMainText.TabIndex = 0;
            this.lblMainText.Text = "lblMainText";
            // 
            // lblProjectDir
            // 
            this.lblProjectDir.AutoSize = true;
            this.lblProjectDir.Location = new System.Drawing.Point(12, 79);
            this.lblProjectDir.Name = "lblProjectDir";
            this.lblProjectDir.Size = new System.Drawing.Size(63, 13);
            this.lblProjectDir.TabIndex = 1;
            this.lblProjectDir.Text = "lblProjectDir";
            // 
            // txtAssemblyFile
            // 
            this.txtAssemblyFile.Location = new System.Drawing.Point(15, 95);
            this.txtAssemblyFile.Name = "txtAssemblyFile";
            this.txtAssemblyFile.Size = new System.Drawing.Size(406, 20);
            this.txtAssemblyFile.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(427, 93);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cboMarkOutputColor);
            this.grpOptions.Controls.Add(this.chkMarkOutput);
            this.grpOptions.Controls.Add(this.chkEncodeDerivedControls);
            this.grpOptions.Controls.Add(this.chkDoubleEncoding);
            this.grpOptions.Location = new System.Drawing.Point(15, 351);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(568, 116);
            this.grpOptions.TabIndex = 4;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "grpOptions";
            // 
            // cboMarkOutputColor
            // 
            this.cboMarkOutputColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarkOutputColor.FormattingEnabled = true;
            this.cboMarkOutputColor.Location = new System.Drawing.Point(7, 89);
            this.cboMarkOutputColor.Name = "cboMarkOutputColor";
            this.cboMarkOutputColor.Size = new System.Drawing.Size(247, 21);
            this.cboMarkOutputColor.Sorted = true;
            this.cboMarkOutputColor.TabIndex = 1;
            // 
            // chkMarkOutput
            // 
            this.chkMarkOutput.AutoSize = true;
            this.chkMarkOutput.Location = new System.Drawing.Point(7, 66);
            this.chkMarkOutput.Name = "chkMarkOutput";
            this.chkMarkOutput.Size = new System.Drawing.Size(100, 17);
            this.chkMarkOutput.TabIndex = 0;
            this.chkMarkOutput.Text = "chkMarkOutput";
            this.chkMarkOutput.UseVisualStyleBackColor = true;
            // 
            // chkEncodeDerivedControls
            // 
            this.chkEncodeDerivedControls.AutoSize = true;
            this.chkEncodeDerivedControls.Checked = true;
            this.chkEncodeDerivedControls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEncodeDerivedControls.Location = new System.Drawing.Point(7, 43);
            this.chkEncodeDerivedControls.Name = "chkEncodeDerivedControls";
            this.chkEncodeDerivedControls.Size = new System.Drawing.Size(156, 17);
            this.chkEncodeDerivedControls.TabIndex = 0;
            this.chkEncodeDerivedControls.Text = "chkEncodeDerivedControls";
            this.chkEncodeDerivedControls.UseVisualStyleBackColor = true;
            // 
            // chkDoubleEncoding
            // 
            this.chkDoubleEncoding.AutoSize = true;
            this.chkDoubleEncoding.Checked = true;
            this.chkDoubleEncoding.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDoubleEncoding.Location = new System.Drawing.Point(7, 20);
            this.chkDoubleEncoding.Name = "chkDoubleEncoding";
            this.chkDoubleEncoding.Size = new System.Drawing.Size(89, 17);
            this.chkDoubleEncoding.TabIndex = 0;
            this.chkDoubleEncoding.Text = "chkEncoding";
            this.chkDoubleEncoding.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(427, 473);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "btnGenerate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(508, 473);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(508, 93);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyze.TabIndex = 6;
            this.btnAnalyze.Text = "btnAnalyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // grpAnalysis
            // 
            this.grpAnalysis.Controls.Add(this.lvwAnalysis);
            this.grpAnalysis.Location = new System.Drawing.Point(15, 121);
            this.grpAnalysis.Name = "grpAnalysis";
            this.grpAnalysis.Size = new System.Drawing.Size(568, 224);
            this.grpAnalysis.TabIndex = 7;
            this.grpAnalysis.TabStop = false;
            this.grpAnalysis.Text = "grpAnalysis";
            // 
            // lvwAnalysis
            // 
            this.lvwAnalysis.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnPage,
            this.clmnControl,
            this.clmnControlType});
            this.lvwAnalysis.FullRowSelect = true;
            this.lvwAnalysis.Location = new System.Drawing.Point(7, 19);
            this.lvwAnalysis.Name = "lvwAnalysis";
            this.lvwAnalysis.Size = new System.Drawing.Size(555, 194);
            this.lvwAnalysis.TabIndex = 0;
            this.lvwAnalysis.UseCompatibleStateImageBehavior = false;
            this.lvwAnalysis.View = System.Windows.Forms.View.Details;
            // 
            // clmnPage
            // 
            this.clmnPage.Text = "clmnPage";
            this.clmnPage.Width = 150;
            // 
            // clmnControl
            // 
            this.clmnControl.Text = "clmnControl";
            this.clmnControl.Width = 200;
            // 
            // clmnControlType
            // 
            this.clmnControlType.Text = "clmnControlType";
            this.clmnControlType.Width = 200;
            // 
            // dlgAssembly
            // 
            this.dlgAssembly.Title = "Select a web application assembly";
            // 
            // dlgGenConfig
            // 
            this.dlgGenConfig.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 508);
            this.Controls.Add(this.grpAnalysis);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtAssemblyFile);
            this.Controls.Add(this.lblProjectDir);
            this.Controls.Add(this.lblMainText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.grpAnalysis.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMainText;
        private System.Windows.Forms.Label lblProjectDir;
        private System.Windows.Forms.TextBox txtAssemblyFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkDoubleEncoding;
        private System.Windows.Forms.CheckBox chkEncodeDerivedControls;
        private System.Windows.Forms.CheckBox chkMarkOutput;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.GroupBox grpAnalysis;
        private System.Windows.Forms.ListView lvwAnalysis;
        private System.Windows.Forms.ColumnHeader clmnPage;
        private System.Windows.Forms.ColumnHeader clmnControl;
        private System.Windows.Forms.ColumnHeader clmnControlType;
        private System.Windows.Forms.OpenFileDialog dlgAssembly;
        private System.Windows.Forms.FolderBrowserDialog dlgGenConfig;
        private System.Windows.Forms.HelpProvider hlpControls;
        private System.Windows.Forms.ComboBox cboMarkOutputColor;
    }
}

