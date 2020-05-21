using System;
using System.Drawing;
using TDAmeritradeZorro.Classes;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.WebApi
{
    //*************************************************************************
    //  Class: AuthForm
    //
    /// <summary>
    /// This is the designer class behind the Windows Form that shows a web
    /// browser control for interacting with the TD Ameritrade HTML servers.
    /// </summary>
    //*************************************************************************
    partial class AuthForm
    {
        //*********************************************************************
        //  Member: IContainer
        //
        /// <summary>
        /// Required designer variable.
        /// </summary>
        //*********************************************************************
        private System.ComponentModel.IContainer components = null;

        //*********************************************************************
        //  Member: webBrowser
        //
        /// <summary>
        /// The web browser control that occupies this Windows Form
        /// </summary>
        //*********************************************************************
        public System.Windows.Forms.WebBrowser webBrowser;

        //*********************************************************************
        //  Member: icon
        //
        /// <summary>
        /// The icon for the Web Browser Form window.
        /// </summary>
        //*********************************************************************
        public Icon icon;

        //*********************************************************************
        //  Method: Dispose
        //
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// 
        /// <param name="disposing">
        /// True if managed resources should be disposed; otherwise, false.
        /// </param>
        //*********************************************************************
        protected override void 
            Dispose
            (
            bool disposing
            )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        //*********************************************************************
        //  Method: InitializeComponent
        //
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        //*********************************************************************
        private void InitializeComponent()
        {
            try
            {
                // Enable the web browser to support IE Edge emulation, 
                // including the use of modern Javascript listeners
                EnsureBrowserEmulationEnabled();

                // Create the icon for the Windows Form
                icon = Helper.GetWindowsFormIcon(TDAmeritradeZorro.Classes.FormType.Auth);

                // Width and height of the form and th web browser control 
                int width = 475;
                int height = 450;

                // Initialize the web browser control
                this.webBrowser = new System.Windows.Forms.WebBrowser();

                // Suspend layout while initializing form and browser
                this.SuspendLayout();

                // 
                // webBrowser
                // 
                this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
                this.webBrowser.Location = new System.Drawing.Point(0, 0);
                this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
                this.webBrowser.Name = "webBrowser";
                this.webBrowser.Size = new System.Drawing.Size(width, height);
                this.webBrowser.TabIndex = 0;
                this.webBrowser.ScrollBarsEnabled = false;

                // 
                // AuthForm
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(width, height);
                this.Controls.Add(this.webBrowser);
                this.Name = "TD Ameritrade";
                this.Text = "TD Ameritrade";
                this.Icon = icon;

                // Resume laying out form
                this.ResumeLayout(false);
            }
            catch (Exception e)
            {
                // Log thet error
                LogHelper.Log(LogLevel.Error, e.Message);
            }
        }
        #endregion
    }
}