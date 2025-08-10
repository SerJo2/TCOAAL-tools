namespace TCOAAL_tools
{
    partial class Form1
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
            OpenGame = new Button();
            Save = new Button();
            Category = new GroupBox();
            Any = new RadioButton();
            GameStatus = new Label();
            InstallPlugin = new Button();
            Burial = new RadioButton();
            Incest = new RadioButton();
            Andy = new RadioButton();
            Category.SuspendLayout();
            SuspendLayout();
            // 
            // OpenGame
            // 
            OpenGame.Location = new Point(167, 12);
            OpenGame.Name = "OpenGame";
            OpenGame.Size = new Size(122, 27);
            OpenGame.TabIndex = 0;
            OpenGame.Text = "Open Folder...";
            OpenGame.UseVisualStyleBackColor = true;
            OpenGame.Click += OpenGame_Click;
            // 
            // Save
            // 
            Save.Enabled = false;
            Save.Location = new Point(167, 147);
            Save.Name = "Save";
            Save.Size = new Size(122, 71);
            Save.TabIndex = 1;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // Category
            // 
            Category.Controls.Add(Andy);
            Category.Controls.Add(Incest);
            Category.Controls.Add(Burial);
            Category.Controls.Add(Any);
            Category.Location = new Point(12, 12);
            Category.Name = "Category";
            Category.Size = new Size(149, 151);
            Category.TabIndex = 5;
            Category.TabStop = false;
            Category.Text = "Category";
            Category.Enter += groupBox1_Enter;
            // 
            // Any
            // 
            Any.AutoSize = true;
            Any.Enabled = false;
            Any.Location = new Point(6, 22);
            Any.Name = "Any";
            Any.Size = new Size(56, 19);
            Any.TabIndex = 6;
            Any.TabStop = true;
            Any.Text = "Any%";
            Any.UseVisualStyleBackColor = true;
            Any.CheckedChanged += Any_CheckedChanged;
            // 
            // GameStatus
            // 
            GameStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GameStatus.AutoEllipsis = true;
            GameStatus.Font = new Font("Segoe UI", 11F);
            GameStatus.Location = new Point(12, 166);
            GameStatus.Name = "GameStatus";
            GameStatus.Size = new Size(142, 52);
            GameStatus.TabIndex = 6;
            GameStatus.Text = "No game";
            GameStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // InstallPlugin
            // 
            InstallPlugin.Enabled = false;
            InstallPlugin.Location = new Point(167, 45);
            InstallPlugin.Name = "InstallPlugin";
            InstallPlugin.Size = new Size(122, 23);
            InstallPlugin.TabIndex = 7;
            InstallPlugin.Text = "Install Plugin";
            InstallPlugin.UseVisualStyleBackColor = true;
            InstallPlugin.Click += InstallPlugin_Click;
            // 
            // Burial
            // 
            Burial.AutoSize = true;
            Burial.Enabled = false;
            Burial.Location = new Point(6, 47);
            Burial.Name = "Burial";
            Burial.Size = new Size(55, 19);
            Burial.TabIndex = 7;
            Burial.TabStop = true;
            Burial.Text = "Burial";
            Burial.UseVisualStyleBackColor = true;
            Burial.CheckedChanged += Burial_CheckedChanged;
            // 
            // Incest
            // 
            Incest.AutoSize = true;
            Incest.Enabled = false;
            Incest.Location = new Point(6, 72);
            Incest.Name = "Incest";
            Incest.Size = new Size(66, 19);
            Incest.TabIndex = 8;
            Incest.TabStop = true;
            Incest.Text = "Incest%";
            Incest.UseVisualStyleBackColor = true;
            Incest.CheckedChanged += Incest_CheckedChanged;
            // 
            // Andy
            // 
            Andy.AutoSize = true;
            Andy.Enabled = false;
            Andy.Location = new Point(6, 97);
            Andy.Name = "Andy";
            Andy.Size = new Size(53, 19);
            Andy.TabIndex = 9;
            Andy.TabStop = true;
            Andy.Text = "Andy";
            Andy.UseVisualStyleBackColor = true;
            Andy.CheckedChanged += Andy_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(298, 227);
            Controls.Add(InstallPlugin);
            Controls.Add(GameStatus);
            Controls.Add(Category);
            Controls.Add(Save);
            Controls.Add(OpenGame);
            Name = "Form1";
            Text = "TCOAAL Tools";
            Load += Form1_Load;
            Category.ResumeLayout(false);
            Category.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button OpenGame;
        private Button Save;
        private GroupBox Category;
        private RadioButton Any;
        private Label GameStatus;
        private Button InstallPlugin;
        private RadioButton Andy;
        private RadioButton Incest;
        private RadioButton Burial;
    }
}
