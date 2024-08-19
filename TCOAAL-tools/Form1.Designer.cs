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
            AllAchiv = new RadioButton();
            Any = new RadioButton();
            Incest = new RadioButton();
            Category.SuspendLayout();
            SuspendLayout();
            // 
            // OpenGame
            // 
            OpenGame.Location = new Point(167, 12);
            OpenGame.Name = "OpenGame";
            OpenGame.Size = new Size(122, 27);
            OpenGame.TabIndex = 0;
            OpenGame.Text = "Open Game";
            OpenGame.UseVisualStyleBackColor = true;
            OpenGame.Click += OpenGame_Click;
            // 
            // Save
            // 
            Save.Enabled = false;
            Save.Location = new Point(167, 45);
            Save.Name = "Save";
            Save.Size = new Size(122, 71);
            Save.TabIndex = 1;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // Category
            // 
            Category.Controls.Add(AllAchiv);
            Category.Controls.Add(Any);
            Category.Controls.Add(Incest);
            Category.Location = new Point(12, 12);
            Category.Name = "Category";
            Category.Size = new Size(149, 104);
            Category.TabIndex = 5;
            Category.TabStop = false;
            Category.Text = "Category";
            Category.Enter += groupBox1_Enter;
            // 
            // AllAchiv
            // 
            AllAchiv.AutoSize = true;
            AllAchiv.Enabled = false;
            AllAchiv.Location = new Point(6, 72);
            AllAchiv.Name = "AllAchiv";
            AllAchiv.Size = new Size(111, 19);
            AllAchiv.TabIndex = 8;
            AllAchiv.TabStop = true;
            AllAchiv.Text = "All Achivements";
            AllAchiv.UseVisualStyleBackColor = true;
            AllAchiv.CheckedChanged += AllAchiv_CheckedChanged;
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
            // Incest
            // 
            Incest.AutoSize = true;
            Incest.Enabled = false;
            Incest.Location = new Point(6, 47);
            Incest.Name = "Incest";
            Incest.Size = new Size(56, 19);
            Incest.TabIndex = 7;
            Incest.TabStop = true;
            Incest.Text = "Incest";
            Incest.UseVisualStyleBackColor = true;
            Incest.CheckedChanged += Incest_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 131);
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
        private RadioButton AllAchiv;
        private RadioButton Any;
        private RadioButton Incest;
    }
}
