using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class TreeBrowseForm: BrowseForm, ICompiledEntity
	{
		public TreeBrowseForm(){}

		public TreeBrowseForm(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string DisplayMember = "";
		public string SortMember = "";
		public System.String PhisicalName = "";

		#region Ìועמהû
		#region GenerateFormText
		public override string GenerateFormText()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string baseText = base.GenerateFormText();
            sb.Append("InitializeComponent();");
            if (!string.IsNullOrEmpty(DisplayMember))
            {
                sb.Append("\r\n\t\t\t((ObjectTree)this.ObjectContainer).DisplayMember = \"");
                sb.Append(DisplayMember);
                sb.Append("\";");
            }
            if (!string.IsNullOrEmpty(SortMember))
            {
                sb.Append("\r\n\t\t\t((ObjectTree)this.ObjectContainer).SortMember = \"");
                sb.Append(SortMember);
                sb.Append("\";");
            }
            string newText = baseText.Replace("InitializeComponent();", sb.ToString())
                .Replace(": GridBrowser", ": TreeBrowser")
                .Replace("this.Obj = new Obj(", "this.Obj = new TreeObj(");
            return newText;



		}
		#endregion
		#endregion
	}
}
