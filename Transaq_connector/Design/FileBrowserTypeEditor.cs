using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using SmartQuant;

namespace OpenQuant.Finam.Design
{
	public class FileBrowserTypeEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
				if (string.IsNullOrWhiteSpace((string)value))
				{
					folderBrowserDialog.SelectedPath = Framework.get_Installation().get_LogDir().FullName;
				}
				folderBrowserDialog.SelectedPath = (string)value;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					return folderBrowserDialog.SelectedPath;
				}
				return Framework.get_Installation().get_LogDir().FullName;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
