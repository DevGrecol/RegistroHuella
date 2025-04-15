namespace PruebaDigitalPersonRegistrar
{
    partial class frmVerificar
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelNumeroPago = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEncontradoNombre = new System.Windows.Forms.TextBox();
            this.txtEncontradoApellido = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEncontradoCedula = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelNumeroPago);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtEncontradoNombre);
            this.groupBox1.Controls.Add(this.txtEncontradoApellido);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtEncontradoCedula);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(266, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 235);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Datos del Cliente";
            // 
            // labelNumeroPago
            // 
            this.labelNumeroPago.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelNumeroPago.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNumeroPago.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.labelNumeroPago.Location = new System.Drawing.Point(117, 165);
            this.labelNumeroPago.Name = "labelNumeroPago";
            this.labelNumeroPago.Size = new System.Drawing.Size(162, 49);
            this.labelNumeroPago.TabIndex = 8;
            this.labelNumeroPago.Text = "XXX";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Codigo Para pago:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // txtEncontradoNombre
            // 
            this.txtEncontradoNombre.Location = new System.Drawing.Point(62, 34);
            this.txtEncontradoNombre.Name = "txtEncontradoNombre";
            this.txtEncontradoNombre.Size = new System.Drawing.Size(209, 20);
            this.txtEncontradoNombre.TabIndex = 8;
            this.txtEncontradoNombre.TextChanged += new System.EventHandler(this.txtEncontradoNombre_TextChanged);
            // 
            // txtEncontradoApellido
            // 
            this.txtEncontradoApellido.Location = new System.Drawing.Point(62, 77);
            this.txtEncontradoApellido.Name = "txtEncontradoApellido";
            this.txtEncontradoApellido.Size = new System.Drawing.Size(209, 20);
            this.txtEncontradoApellido.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Cedula:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtEncontradoCedula
            // 
            this.txtEncontradoCedula.Location = new System.Drawing.Point(62, 133);
            this.txtEncontradoCedula.Name = "txtEncontradoCedula";
            this.txtEncontradoCedula.Size = new System.Drawing.Size(209, 20);
            this.txtEncontradoCedula.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Apellido:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre:";
            // 
            // frmVerificar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 442);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmVerificar";
            this.Text = "frmVerificar";
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEncontradoNombre;
        private System.Windows.Forms.TextBox txtEncontradoApellido;
        private System.Windows.Forms.TextBox txtEncontradoCedula;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelNumeroPago;
    }
}