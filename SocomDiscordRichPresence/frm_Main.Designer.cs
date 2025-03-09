namespace SocomDiscordRichPresence
{
    partial class frm_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tmr_GameScan = new System.Windows.Forms.Timer(components);
            pct_CurrentGame = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pct_CurrentGame).BeginInit();
            SuspendLayout();
            // 
            // tmr_GameScan
            // 
            tmr_GameScan.Enabled = true;
            tmr_GameScan.Interval = 1000;
            tmr_GameScan.Tick += tmr_GameScan_Tick;
            // 
            // pct_CurrentGame
            // 
            pct_CurrentGame.Location = new Point(0, 0);
            pct_CurrentGame.Name = "pct_CurrentGame";
            pct_CurrentGame.Size = new Size(256, 128);
            pct_CurrentGame.TabIndex = 0;
            pct_CurrentGame.TabStop = false;
            // 
            // frm_Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(257, 127);
            Controls.Add(pct_CurrentGame);
            MaximumSize = new Size(273, 166);
            MinimumSize = new Size(273, 166);
            Name = "frm_Main";
            ((System.ComponentModel.ISupportInitialize)pct_CurrentGame).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer tmr_GameScan;
        private PictureBox pct_CurrentGame;
    }
}
