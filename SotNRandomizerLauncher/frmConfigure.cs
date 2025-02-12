﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace SotNRandomizerLauncher
{
    public partial class frmConfigure : Form
    {

        bool initialSetup = true;
        public bool importConfirmed = false;
        bool classicCoreInstalled = true;
        public frmConfigure()
        {
            InitializeComponent();
            LoadFromConfig();
            CheckFields();
        }

        private void lblDescription_Click(object sender, EventArgs e)
        {

        }

        private void btnUploadBios_Click(object sender, EventArgs e)
        {
            string filePath = LauncherClient.RequestFile("scph7003.bin", "bin");
            if (filePath == "") return;
            txtBiosPath.Text = filePath;
            LauncherClient.SetAppConfig("BiosPath", filePath);
            CheckFields();
        }

        private void CheckFields()
        {
            if(txtBiosPath.Text != "" && txtCuePath.Text != "" && txtTrack1Path.Text != "" && txtTrack2Path.Text != "")
            {
                btnContinue.Enabled = true;
            }
        }

        private void ChangeCoreData()
        {
            if (LauncherClient.GetConfigValue("CoreInstalled") == "ClassicCore")
            {
                btnChangeCore.Text = "Change to Fast Core";
                lblCurrentCore.Text = "Core Installed: Classic Core";
                this.classicCoreInstalled = true;
            }
            else
            {
                btnChangeCore.Text = "Change to Classic Core";
                lblCurrentCore.Text = "Core Installed: Fast Core";
                this.classicCoreInstalled = false;
            }
        }

        private void LoadFromConfig()
        {
            txtBiosPath.Text = GetConfigValue("BiosPath");
            txtCuePath.Text = GetConfigValue("CuePath");
            txtTrack1Path.Text = GetConfigValue("Track1Path");
            txtTrack2Path.Text =  GetConfigValue("Track2Path");
            initialSetup = LauncherClient.IsInitialSetup();
            ChangeCoreData();
            
            if (!initialSetup)
            {
                cbImport.Hide();
                grpEmulation.Show();
                lblDescription.Hide();
            }
            else
            {

            }
        }        

        private string GetConfigValue(string config)
        {
            return LauncherClient.GetConfigValue(config);       
        }

        private void btnUploadTrack1_Click(object sender, EventArgs e)
        {
            string filePath = LauncherClient.RequestFile("Castlevania - Symphony of the Night (USA) (Track 1).bin", "bin");
            if(filePath == "") return;
            txtTrack1Path.Text = filePath;
            LauncherClient.SetAppConfig("Track1Path", filePath);
            CheckFields();
        }

        private void btnUploadTrack2_Click(object sender, EventArgs e)
        {
            string filePath = LauncherClient.RequestFile("Castlevania - Symphony of the Night (USA) (Track 2).bin", "bin");
            if (filePath == "") return;
            txtTrack2Path.Text = filePath;
            LauncherClient.SetAppConfig("Track2Path", filePath);
            CheckFields();
        }

        private void btnUploadCue_Click(object sender, EventArgs e)
        {
            string filePath = LauncherClient.RequestFile("Castlevania - Symphony of the Night (USA).cue", "cue");
            if (filePath == "") return;
            txtCuePath.Text = filePath;
            LauncherClient.SetAppConfig("CuePath", filePath);
            CheckFields();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnContinue_Click(object sender, EventArgs e)
        {
            if (!initialSetup)
            {
                this.Close();
                return;
            }

            if (cbImport.Checked)
            {
                DialogResult result = MessageBox.Show(
                    "Using your own installation of Bizhawk and LiveSplit means that you won't receive automatic updates for these tools. Are you sure you want to continue?",
                    "Import Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if(result != DialogResult.Yes) return;

                ImportProcess();
                if (this.importConfirmed)
                {
                    LauncherClient.InitialSetupDone();
                    this.Close();
                }                
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    "We will now download LiveSplit, BizHawk and RandoTools to play the randomizer. Are you sure you want to continue?",
                    "Confirm Setup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );
                if (result != DialogResult.Yes) return;
                try
                {
                    await NewUserProcess();
                    LauncherClient.InitialSetupDone();
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error while downloading tools: {ex.Message}. Setup cancelled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }                
                this.Close();
            }            
        }

        void StoreCores()
        {
            string bizHawkDirectory = LauncherClient.GetConfigValue("BizHawkPath");
            string currentAppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            LauncherClient.StoreCores(currentAppDirectory, bizHawkDirectory);
        }

        void ImportProcess()
        {
            frmImport frmImport = new frmImport(this);
            frmImport.ShowDialog();
            StoreCores();
        }

        async Task NewUserProcess()
        {
            await LauncherClient.DownloadLiveSplit();
            await LauncherClient.DownloadBizHawk();
            await LauncherClient.DownloadRandoTools();
            StoreCores();
            MessageBox.Show("Setup Successful! You can now use the Randomizer Launcher.", "Setup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnChangeCore_Click(object sender, EventArgs e)
        {
            // If the classic core is installed, swaps to Fast. Else, swaps to Classic.
            LauncherClient.SwapCores(this.classicCoreInstalled);
            ChangeCoreData();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
