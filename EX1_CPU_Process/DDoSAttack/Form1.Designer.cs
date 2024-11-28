namespace DDoSAttack
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
            numOfBrowswers = new TextBox();
            theURL = new TextBox();
            attack_with = new Label();
            browsers = new Label();
            targetUrl = new Label();
            Start = new Button();
            colseAll = new Button();
            SuspendLayout();
            // 
            // numOfBrowswers
            // 
            numOfBrowswers.Location = new Point(134, 50);
            numOfBrowswers.Name = "numOfBrowswers";
            numOfBrowswers.Size = new Size(94, 31);
            numOfBrowswers.TabIndex = 0;
            numOfBrowswers.TextChanged += numOfBrowswers_TextChanged;
            // 
            // theURL
            // 
            theURL.Location = new Point(130, 114);
            theURL.Name = "theURL";
            theURL.Size = new Size(150, 31);
            theURL.TabIndex = 1;
            theURL.TextChanged += theURL_TextChanged;
            // 
            // attack_with
            // 
            attack_with.AutoSize = true;
            attack_with.Location = new Point(24, 50);
            attack_with.Name = "attack_with";
            attack_with.Size = new Size(104, 25);
            attack_with.TabIndex = 2;
            attack_with.Text = "Attack with:";
            attack_with.Click += label1_Click;
            // 
            // browsers
            // 
            browsers.AutoSize = true;
            browsers.Location = new Point(245, 50);
            browsers.Name = "browsers";
            browsers.Size = new Size(83, 25);
            browsers.TabIndex = 3;
            browsers.Text = "Browsers";
            // 
            // targetUrl
            // 
            targetUrl.AutoSize = true;
            targetUrl.Location = new Point(24, 114);
            targetUrl.Name = "targetUrl";
            targetUrl.Size = new Size(100, 25);
            targetUrl.TabIndex = 4;
            targetUrl.Text = "Target URL:";
            // 
            // Start
            // 
            Start.Location = new Point(134, 223);
            Start.Name = "Start";
            Start.Size = new Size(366, 56);
            Start.TabIndex = 5;
            Start.Text = "Start DDos Attack";
            Start.UseVisualStyleBackColor = true;
            Start.Click += Start_Click;
            // 
            // colseAll
            // 
            colseAll.Location = new Point(134, 285);
            colseAll.Name = "colseAll";
            colseAll.Size = new Size(366, 53);
            colseAll.TabIndex = 6;
            colseAll.Text = "Close All";
            colseAll.UseVisualStyleBackColor = true;
            colseAll.Click += colseAll_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(629, 398);
            Controls.Add(colseAll);
            Controls.Add(Start);
            Controls.Add(targetUrl);
            Controls.Add(browsers);
            Controls.Add(attack_with);
            Controls.Add(theURL);
            Controls.Add(numOfBrowswers);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox numOfBrowswers;
        private TextBox theURL;
        private Label attack_with;
        private Label browsers;
        private Label targetUrl;
        private Button Start;
        private Button colseAll;
    }
}
