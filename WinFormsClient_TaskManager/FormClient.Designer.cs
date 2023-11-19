namespace Client_TaskManager
{
    partial class FormClient
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
            button1 = new Button();
            listBox1 = new ListBox();
            button4 = new Button();
            button6 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            buttonConnection = new Button();
            ip_address = new TextBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(1, 65);
            button1.Margin = new Padding(4, 4, 4, 4);
            button1.Name = "button1";
            button1.Size = new Size(515, 56);
            button1.TabIndex = 0;
            button1.Text = "Get the list of the processes";
            button1.UseVisualStyleBackColor = true;
            button1.Click += UpdateProcessList_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 20;
            listBox1.Location = new Point(1, 154);
            listBox1.Margin = new Padding(4, 4, 4, 4);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(517, 304);
            listBox1.TabIndex = 1;
            // 
            // button4
            // 
            button4.Location = new Point(1, 573);
            button4.Name = "button4";
            button4.Size = new Size(515, 52);
            button4.TabIndex = 5;
            button4.Text = "Создать новый процесс";
            button4.UseVisualStyleBackColor = true;
            button4.Click += CreateProcess_Click;
            // 
            // button6
            // 
            button6.Location = new Point(1, 464);
            button6.Name = "button6";
            button6.Size = new Size(515, 52);
            button6.TabIndex = 7;
            button6.Text = "Kill the process";
            button6.UseVisualStyleBackColor = true;
            button6.Click += EndProcess_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(181, 125);
            label1.Margin = new Padding(1, 0, 1, 0);
            label1.Name = "label1";
            label1.Size = new Size(142, 20);
            label1.TabIndex = 8;
            label1.Text = "List of the processes";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(1, 534);
            textBox1.Margin = new Padding(1, 1, 1, 1);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(517, 27);
            textBox1.TabIndex = 9;
            // 
            // buttonConnection
            // 
            buttonConnection.Location = new Point(1, 1);
            buttonConnection.Margin = new Padding(4, 4, 4, 4);
            buttonConnection.Name = "buttonConnection";
            buttonConnection.Size = new Size(235, 56);
            buttonConnection.TabIndex = 10;
            buttonConnection.Text = "Connect to Server";
            buttonConnection.UseVisualStyleBackColor = true;
            buttonConnection.Click += buttonConnection_Click;
            // 
            // ip_address
            // 
            ip_address.Location = new Point(241, 30);
            ip_address.Margin = new Padding(1, 1, 1, 1);
            ip_address.Name = "ip_address";
            ip_address.Size = new Size(275, 27);
            ip_address.TabIndex = 11;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(309, 1);
            label2.Margin = new Padding(1, 0, 1, 0);
            label2.Name = "label2";
            label2.Size = new Size(121, 20);
            label2.TabIndex = 12;
            label2.Text = "Server IP address";
            // 
            // FormClient
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(519, 515);
            Controls.Add(label2);
            Controls.Add(ip_address);
            Controls.Add(buttonConnection);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button6);
            Controls.Add(button4);
            Controls.Add(listBox1);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 4, 4, 4);
            MaximizeBox = false;
            Name = "FormClient";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Processes";
            FormClosed += FormClient_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private ListBox listBox1;
        private Button button4;
        private Button button6;
        private Label label1;
        private TextBox textBox1;
        private Button buttonConnection;
        private TextBox ip_address;
        private Label label2;
    }
}

