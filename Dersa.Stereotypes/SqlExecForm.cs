using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class SqlExecForm: StereotypeBaseE, ICompiledEntity
	{
		public SqlExecForm(){}

		public SqlExecForm(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region Exec
		public static void Exec(string text, string serverName, string databaseName)
		{
return;
			/*
			SqlExecForm form = new SqlExecForm();
			
			System.Windows.Forms.StatusBar statusBar = new System.Windows.Forms.StatusBar();
			System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
			System.Windows.Forms.Button bExecute = new System.Windows.Forms.Button();
			System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
			
			Modeler.Editor.TextSource textSource = new Modeler.Editor.TextSource();
			Modeler.Editor.Parser parser = new Modeler.Editor.Parser();
			panel.SuspendLayout();
			form.SuspendLayout();
			// 
			// statusBar
			// 
			statusBar.Location = new System.Drawing.Point(0, 447);
			statusBar.Name = "statusBar";
			statusBar.Size = new System.Drawing.Size(672, 22);
			statusBar.TabIndex = 2;
			// 
			// panel
			// 
			panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			panel.Controls.Add(form.tbPassword);
			panel.Controls.Add(form.tbLogin);
			panel.Controls.Add(form.tbServer);
			panel.Controls.Add(form.tbDatabase);
			panel.Controls.Add(bExecute);
			panel.Dock = System.Windows.Forms.DockStyle.Top;
			panel.Location = new System.Drawing.Point(0, 0);
			panel.Name = "panel";
			panel.Size = new System.Drawing.Size(672, 30);
			panel.TabIndex = 5;
			// 
			// bExecute
			// 
			bExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			bExecute.FlatStyle = System.Windows.Forms.FlatStyle.System;
			bExecute.Location = new System.Drawing.Point(592, 2);
			bExecute.Name = "bExecute";
			bExecute.TabIndex = 9;
			bExecute.Text = "Execute";
			bExecute.Click += new System.EventHandler(form.ExecSqlClick);
			
			Microsoft.Win32.RegistryKey key = form.GetRegistryKey();
			// 
			// eServer
			// 
			form.tbServer.Location = new System.Drawing.Point(8, 3);
			form.tbServer.Name = "tbServer";
			form.tbServer.TabIndex = 5;
			string text_value = null;
			if (serverName != null)
				text_value = serverName;
			else
				text_value = key.GetValue(form.tbServer.Name) as string;
			if (text_value != null)
				form.tbServer.Text = text_value;
			else
				form.tbServer.Text = "";
			toolTip.SetToolTip(form.tbServer, "Наименование сервера");
			// 
			// eDataBase
			// 
			form.tbDatabase.Location = new System.Drawing.Point(112, 3);
			form.tbDatabase.Name = "tbDatabase";
			form.tbDatabase.TabIndex = 6;
			if (databaseName != null)
				text_value = databaseName;
			else
				text_value = key.GetValue(form.tbDatabase.Name) as string;
			if (text_value != null)
				form.tbDatabase.Text = text_value;
			else
				form.tbDatabase.Text = "";
			toolTip.SetToolTip(form.tbDatabase, "Наименование базы данных");
			// 
			// tbLogin
			// 
			form.tbLogin.Location = new System.Drawing.Point(224, 3);
			form.tbLogin.Name = "tbLogin";
			form.tbLogin.TabIndex = 7;
			text_value = key.GetValue(form.tbLogin.Name) as string;
			if (text_value != null)
			{
				form.tbLogin.Text = text_value;
			}
			else
			{
				form.tbLogin.Text = "";
			}
			toolTip.SetToolTip(form.tbLogin, "Имя пользователя");
			// 
			// tbPassword
			// 
			form.tbPassword.Location = new System.Drawing.Point(328, 3);
			form.tbPassword.Name = "tbPassword";
			form.tbPassword.PasswordChar = '*';
			form.tbPassword.TabIndex = 8;
			text_value = key.GetValue(form.tbPassword.Name) as string;
			if (text_value != null)
			{
				form.tbPassword.Text = text_value;
			}
			else
			{
				form.tbPassword.Text = "";
			}
			toolTip.SetToolTip(form.tbPassword, "Пароль");
			// 
			// syntaxEdit
			// 
			form.syntaxEdit.BackColor = System.Drawing.SystemColors.Window;
			form.syntaxEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			form.syntaxEdit.Braces.BracesColor = System.Drawing.Color.Blue;
			form.syntaxEdit.Braces.BracesStyle = System.Drawing.FontStyle.Bold;
			form.syntaxEdit.Braces.ClosingBraces = new char[] {
																  ')',
																  ']',
																  '}'};
			form.syntaxEdit.Braces.OpenBraces = new char[] {
															   '(',
															   '[',
															   '{'};
			form.syntaxEdit.Cursor = System.Windows.Forms.Cursors.IBeam;
			form.syntaxEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			form.syntaxEdit.Font = new System.Drawing.Font("Courier New", 10F);
			form.syntaxEdit.Gutter.BrushColor = System.Drawing.Color.LightGray;
			form.syntaxEdit.Gutter.LineBookmarksColor = System.Drawing.Color.Red;
			form.syntaxEdit.Gutter.LineNumbersAlignment = System.Drawing.StringAlignment.Near;
			form.syntaxEdit.Gutter.LineNumbersBackColor = System.Drawing.SystemColors.Window;
			form.syntaxEdit.Gutter.LineNumbersForeColor = System.Drawing.SystemColors.WindowText;
			form.syntaxEdit.Gutter.Options = Modeler.Editor.GutterOptions.PaintBookMarks;
			form.syntaxEdit.Gutter.PenColor = System.Drawing.Color.Gray;
			form.syntaxEdit.HyperText.UrlColor = System.Drawing.Color.Blue;
			form.syntaxEdit.HyperText.UrlStyle = System.Drawing.FontStyle.Underline;
			form.syntaxEdit.Lexer = parser;
			form.syntaxEdit.LineSeparator.HighlightColor = System.Drawing.SystemColors.ScrollBar;
			form.syntaxEdit.LineSeparator.LineColor = System.Drawing.SystemColors.ControlText;
			form.syntaxEdit.LineSeparator.Options = Modeler.Editor.SeparatorOptions.None;
			form.syntaxEdit.Location = new System.Drawing.Point(0, 30);
			form.syntaxEdit.Margin.PenColor = System.Drawing.Color.Gray;
			form.syntaxEdit.Name = "syntaxEdit";
			form.syntaxEdit.NavigateOptions = ((Modeler.Editor.NavigateOptions)((Modeler.Editor.NavigateOptions.UpAtLineBegin | Modeler.Editor.NavigateOptions.DownAtLineEnd)));
			form.syntaxEdit.Outlining.OutlineColor = System.Drawing.Color.Gray;
			form.syntaxEdit.Outlining.OutlineOptions = ((Modeler.Editor.OutlineOptions)(((Modeler.Editor.OutlineOptions.DrawLines | Modeler.Editor.OutlineOptions.DrawButtons) 
				| Modeler.Editor.OutlineOptions.ShowHints)));
			form.syntaxEdit.Printing.Options = ((Modeler.Editor.PrintOptions)((((((((Modeler.Editor.PrintOptions.LineNumbers | Modeler.Editor.PrintOptions.PageNumbers) 
				| Modeler.Editor.PrintOptions.WordWrap) 
				| Modeler.Editor.PrintOptions.UseColors) 
				| Modeler.Editor.PrintOptions.UseSyntax) 
				| Modeler.Editor.PrintOptions.UseHeader) 
				| Modeler.Editor.PrintOptions.UseFooter) 
				| Modeler.Editor.PrintOptions.DisplayProgress)));
			form.syntaxEdit.Scrolling.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
			form.syntaxEdit.Selection.BackColor = System.Drawing.SystemColors.Highlight;
			form.syntaxEdit.Selection.ForeColor = System.Drawing.SystemColors.HighlightText;
			form.syntaxEdit.Selection.InActiveBackColor = System.Drawing.SystemColors.InactiveCaption;
			form.syntaxEdit.Selection.InActiveForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			form.syntaxEdit.Selection.Options = Modeler.Editor.SelectionOptions.OverwriteBlocks;
			form.syntaxEdit.Size = new System.Drawing.Size(672, 417);
			form.syntaxEdit.Spelling.SpellColor = System.Drawing.Color.Red;
			form.syntaxEdit.TabIndex = 6;
			form.syntaxEdit.WhiteSpace.SymbolColor = System.Drawing.Color.Teal;
			
			if (text.Length > 0)
			{
				text = "set quoted_identifier off \n\n" + text;
			}
			
			form.syntaxEdit.Text = text;
			// 
			// parser
			// 
			parser.Strings = null;
			System.IO.Stream stream = System.Reflection.Assembly.GetAssembly(typeof(Modeler.Editor.SyntaxEdit)).GetManifestResourceStream("Modeler.Editor.Schemes.sql.xml");
			System.IO.TextReader reader = new System.IO.StreamReader(stream);
			parser.LoadScheme(reader);
			// 
			// SqlExecForm
			// 
			form.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			
			string size = key.GetValue("Size") as string;
			if (size != null)
			{
				System.Drawing.SizeConverter sc = new System.Drawing.SizeConverter();
				form.ClientSize = (System.Drawing.Size)sc.ConvertFromString(size);
			}
			else
			{
				form.ClientSize = new System.Drawing.Size(672, 469);
			}
			form.Controls.Add(form.syntaxEdit);
			form.Controls.Add(panel);
			form.Controls.Add(statusBar);
			form.Name = "SqlExecForm";
			form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			form.Text = "Вызов SQL";
			form.Closed+= new EventHandler(form.FormClosed);
			panel.ResumeLayout(false);
			form.ResumeLayout(false);
			
			form.Show();
			form.syntaxEdit.Scrolling.UpdateScroll();
			*/
		}
		#endregion
		#region Exec
		public static void Exec(string text)
		{
//Exec(text, null, null);
		}
		#endregion
		#endregion
	}
}
