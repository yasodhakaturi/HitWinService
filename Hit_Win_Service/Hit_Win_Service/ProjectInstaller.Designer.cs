namespace Hit_Win_Service
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
            this.Hit_Win_Service = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller_Hit_Win_Service = new System.ServiceProcess.ServiceInstaller();
            // 
            // Hit_Win_Service
            // 
            this.Hit_Win_Service.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.Hit_Win_Service.Password = null;
            this.Hit_Win_Service.Username = null;
            this.Hit_Win_Service.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller_Hit_win_Service_AfterInstall);
            // 
            // serviceInstaller_Hit_Win_Service
            // 
            this.serviceInstaller_Hit_Win_Service.DisplayName = "Hit_Win_Service";
            this.serviceInstaller_Hit_Win_Service.ServiceName = "Hit_Win_Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.Hit_Win_Service,
            this.serviceInstaller_Hit_Win_Service});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller Hit_Win_Service;
        private System.ServiceProcess.ServiceInstaller serviceInstaller_Hit_Win_Service;
    }
}