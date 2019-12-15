using System;  
using System.Runtime.Serialization;
using Modeler.Interfaces;
using System.Windows.Forms;

namespace ModelerStereotypes
{
[Serializable()]
public class InputBox: System.Windows.Forms.Form
{
	public InputBox()
	{
	}
	#region Наследуемые свойства
	#endregion
	#region Наследуемые методы
	#endregion

	#region Атрибуты
	#region bCancel
	private System.Windows.Forms.Button bCancel;
	#endregion
	#region bOk
	private System.Windows.Forms.Button bOk;
	#endregion
	#region box
	private static InputBox box;
	#endregion
	#region lText
	private System.Windows.Forms.Label lText;
	#endregion
	#region tbValue
	private System.Windows.Forms.TextBox tbValue;
	#endregion
	#endregion

	#region Операции
	private void InitializeComponent()
	{
			this.tbValue = new System.Windows.Forms.TextBox();
			this.bOk = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.lText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbValue
			// 
			this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbValue.Location = new System.Drawing.Point(8, 32);
			this.tbValue.Name = "tbValue";
			this.tbValue.Size = new System.Drawing.Size(314, 20);
			this.tbValue.TabIndex = 6;
			this.tbValue.Text = "";
			// 
			// bOk
			// 
			this.bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOk.Location = new System.Drawing.Point(168, 64);
			this.bOk.Name = "bOk";
			this.bOk.TabIndex = 7;
			this.bOk.Text = "ОК";
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(248, 64);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 8;
			this.bCancel.Text = "Отмена";
			// 
			// lText
			// 
			this.lText.Location = new System.Drawing.Point(8, 8);
			this.lText.Name = "lText";
			this.lText.Size = new System.Drawing.Size(314, 16);
			this.lText.TabIndex = 9;
			this.lText.Text = "label sfjklndfjkln";
			// 
			// InputBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(330, 96);
			this.Controls.Add(this.lText);
			this.Controls.Add(this.tbValue);
			this.Controls.Add(this.bOk);
			this.Controls.Add(this.bCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.AcceptButton = this.bOk;
			this.CancelButton = this.bCancel;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(176, 128);
			this.Name = "InputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Введите значение";
			this.Load += new System.EventHandler(this.InputBox_Load);
			this.ResumeLayout(false);
	}
	private void InputBox_Load(object sender, System.EventArgs e)
	{
			this.tbValue.Select();
	}
	public static DialogResult Show(string text, string caption, ref string value)
	{
			if (box == null)
			{
				box = new InputBox();
				box.InitializeComponent();
			}
			box.Text = caption;
			box.lText.Text = text + " :";
			System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(box.Handle);
			System.Drawing.SizeF size = g.MeasureString(box.lText.Text, box.lText.Font);
			box.lText.Size = new System.Drawing.Size((int)size.Width + 3, (int)size.Height);
			box.tbValue.Top = box.lText.Top + box.lText.Height + 8;
			box.bOk.Top = box.tbValue.Top + box.tbValue.Height + 12;
			box.bCancel.Top = box.bOk.Top;
			box.ClientSize = new System.Drawing.Size(box.lText.Width + 22, box.bOk.Top+ box.bOk.Height + 12);
		
			box.tbValue.Text = value;
			box.DialogResult = DialogResult.Cancel;
			box.Visible = false;
			DialogResult result = box.ShowDialog();
			value = box.tbValue.Text;
			return result;
	}
	#endregion
}
}