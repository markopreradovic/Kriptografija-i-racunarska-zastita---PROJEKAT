namespace KRZ_Projekat
{
    partial class Prijava
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Prijava));
            label1 = new Label();
            textBox1 = new TextBox();
            label3 = new Label();
            button1 = new Button();
            openFileDialog1 = new OpenFileDialog();
            button2 = new Button();
            label2 = new Label();
            textBox2 = new TextBox();
            label4 = new Label();
            textBox3 = new TextBox();
            button3 = new Button();
            button4 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 40F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(21, 21);
            label1.Name = "label1";
            label1.Size = new Size(200, 72);
            label1.TabIndex = 1;
            label1.Text = "Prijava";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(21, 159);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(338, 23);
            textBox1.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label3.ForeColor = SystemColors.ButtonFace;
            label3.Location = new Point(16, 124);
            label3.Name = "label3";
            label3.Size = new Size(166, 25);
            label3.TabIndex = 4;
            label3.Text = "Digitalni sertifikat";
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button1.Location = new Point(381, 158);
            button1.Name = "button1";
            button1.Size = new Size(45, 23);
            button1.TabIndex = 5;
            button1.Text = ". . .";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button2.Location = new Point(165, 206);
            button2.Name = "button2";
            button2.Size = new Size(96, 31);
            button2.TabIndex = 6;
            button2.Text = "Dalje";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(16, 252);
            label2.Name = "label2";
            label2.Size = new Size(137, 25);
            label2.TabIndex = 8;
            label2.Text = "Korisničko ime";
            label2.Visible = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(21, 287);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(240, 23);
            textBox2.TabIndex = 7;
            textBox2.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label4.ForeColor = SystemColors.ButtonFace;
            label4.Location = new Point(16, 324);
            label4.Name = "label4";
            label4.Size = new Size(77, 25);
            label4.TabIndex = 10;
            label4.Text = "Lozinka";
            label4.Visible = false;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(21, 359);
            textBox3.Name = "textBox3";
            textBox3.PasswordChar = '*';
            textBox3.Size = new Size(240, 23);
            textBox3.TabIndex = 9;
            textBox3.Visible = false;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button3.Location = new Point(21, 409);
            button3.Name = "button3";
            button3.Size = new Size(96, 31);
            button3.TabIndex = 11;
            button3.Text = "Prijava";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button4.Location = new Point(21, 490);
            button4.Name = "button4";
            button4.Size = new Size(132, 31);
            button4.TabIndex = 12;
            button4.Text = "Registracija";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Prijava
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(441, 545);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label4);
            Controls.Add(textBox3);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "Prijava";
            Text = "Prijava";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Label label3;
        private Button button1;
        private OpenFileDialog openFileDialog1;
        private Button button2;
        private Label label2;
        private TextBox textBox2;
        private Label label4;
        private TextBox textBox3;
        private Button button3;
        private Button button4;
    }
}