using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace QuickHull
{
    partial class QuickHullForm
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pb = new System.Windows.Forms.PictureBox();
            btn_Clear = new System.Windows.Forms.Button();
            btn_Apply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pb).BeginInit();
            SuspendLayout();
            // 
            // pb
            // 
            pb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pb.Location = new System.Drawing.Point(12, 12);
            pb.Name = "pb";
            pb.Size = new System.Drawing.Size(606, 426);
            pb.TabIndex = 0;
            pb.TabStop = false;
            pb.Paint += pb_Paint;
            pb.MouseClick += pb_MouseClick;
            // 
            // btn_Clear
            // 
            btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            btn_Clear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btn_Clear.Location = new System.Drawing.Point(624, 12);
            btn_Clear.Name = "btn_Clear";
            btn_Clear.Size = new System.Drawing.Size(164, 47);
            btn_Clear.TabIndex = 1;
            btn_Clear.Text = "Clear";
            btn_Clear.UseVisualStyleBackColor = true;
            btn_Clear.Click += btn_Clear_Click;
            // 
            // btn_Apply
            // 
            btn_Apply.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            btn_Apply.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            btn_Apply.Location = new System.Drawing.Point(624, 65);
            btn_Apply.Name = "btn_Apply";
            btn_Apply.Size = new System.Drawing.Size(164, 47);
            btn_Apply.TabIndex = 2;
            btn_Apply.Text = "Apply QuickHull";
            btn_Apply.UseVisualStyleBackColor = true;
            btn_Apply.Click += btn_Apply_Click;
            // 
            // QuickHullForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(btn_Apply);
            Controls.Add(btn_Clear);
            Controls.Add(pb);
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pb).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Button btn_Apply;

        private System.Windows.Forms.Button btn_Clear;

        private System.Windows.Forms.PictureBox pb;

        #endregion
    }
}