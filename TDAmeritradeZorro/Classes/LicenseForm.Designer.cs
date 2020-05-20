using System.Drawing;
using TDAmeritradeZorro.Utilities;

namespace TDAmeritradeZorro.Classes
{
    partial class LicenseForm
    {
        //*********************************************************************
        //  Member: icon
        //
        /// <summary>
        /// The icon for the Web Browser Form window.
        /// </summary>
        //*********************************************************************
        public Icon icon;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseForm));
            this.Title = new System.Windows.Forms.Label();
            this.SubTitle = new System.Windows.Forms.Label();
            this.LicenseTB = new System.Windows.Forms.RichTextBox();
            this.Agreement = new System.Windows.Forms.Label();
            this.Accept = new System.Windows.Forms.Button();
            this.Decline = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(3, 8);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(153, 20);
            this.Title.TabIndex = 0;
            this.Title.Text = "License Acceptance";
            // 
            // SubTitle
            // 
            this.SubTitle.AutoSize = true;
            this.SubTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SubTitle.Location = new System.Drawing.Point(4, 32);
            this.SubTitle.MaximumSize = new System.Drawing.Size(435, 60);
            this.SubTitle.Name = "SubTitle";
            this.SubTitle.Size = new System.Drawing.Size(433, 45);
            this.SubTitle.TabIndex = 1;
            this.SubTitle.Text = resources.GetString("SubTitle.Text");
            // 
            // LicenseTB
            // 
            this.LicenseTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LicenseTB.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.LicenseTB.Location = new System.Drawing.Point(7, 90);
            this.LicenseTB.Margin = new System.Windows.Forms.Padding(8);
            this.LicenseTB.Name = "LicenseTB";
            this.LicenseTB.ReadOnly = true;
            this.LicenseTB.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.LicenseTB.Size = new System.Drawing.Size(426, 255);
            this.LicenseTB.TabIndex = 2;
            this.LicenseTB.Text = resources.GetString("LicenseTB.Text");
            // 
            // Agreement
            // 
            this.Agreement.AutoSize = true;
            this.Agreement.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Agreement.Location = new System.Drawing.Point(3, 353);
            this.Agreement.MaximumSize = new System.Drawing.Size(440, 40);
            this.Agreement.Name = "Agreement";
            this.Agreement.Size = new System.Drawing.Size(436, 30);
            this.Agreement.TabIndex = 3;
            this.Agreement.Text = "By clicking \"I Agree,\" you agree to the licence terms for this plug-in. If you do" +
    " not agree to the license terms , click \"I Decline.\"";
            // 
            // Accept
            // 
            this.Accept.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Accept.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Accept.Location = new System.Drawing.Point(280, 390);
            this.Accept.Name = "Accept";
            this.Accept.Size = new System.Drawing.Size(75, 23);
            this.Accept.TabIndex = 4;
            this.Accept.Text = "I Accept";
            this.Accept.UseVisualStyleBackColor = true;
            // 
            // Decline
            // 
            this.Decline.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Decline.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Decline.Location = new System.Drawing.Point(358, 390);
            this.Decline.Name = "Decline";
            this.Decline.Size = new System.Drawing.Size(75, 23);
            this.Decline.TabIndex = 5;
            this.Decline.Text = "I Decline";
            this.Decline.UseVisualStyleBackColor = true;
            // 
            // LicenseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 416);
            this.Controls.Add(this.Decline);
            this.Controls.Add(this.Accept);
            this.Controls.Add(this.Agreement);
            this.Controls.Add(this.LicenseTB);
            this.Controls.Add(this.SubTitle);
            this.Controls.Add(this.Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LicenseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "License Acceptance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label SubTitle;
        private System.Windows.Forms.RichTextBox LicenseTB;
        private System.Windows.Forms.Label Agreement;
        private System.Windows.Forms.Button Accept;
        private System.Windows.Forms.Button Decline;
    }
}