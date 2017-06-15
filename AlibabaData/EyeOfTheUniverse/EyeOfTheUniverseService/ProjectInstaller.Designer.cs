namespace EyeOfTheUniverseService
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eyeOfTheUniverseServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.eyeOfTheUniverseServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // eyeOfTheUniverseServiceProcessInstaller
            // 
            this.eyeOfTheUniverseServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.eyeOfTheUniverseServiceProcessInstaller.Password = null;
            this.eyeOfTheUniverseServiceProcessInstaller.Username = null;
            // 
            // eyeOfTheUniverseServiceInstaller
            // 
            this.eyeOfTheUniverseServiceInstaller.ServiceName = "EyeOfTheUniverseService";
            this.eyeOfTheUniverseServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.eyeOfTheUniverseServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.EyeOfTheUniverseServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.eyeOfTheUniverseServiceProcessInstaller,
            this.eyeOfTheUniverseServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller eyeOfTheUniverseServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller eyeOfTheUniverseServiceInstaller;
    }
}