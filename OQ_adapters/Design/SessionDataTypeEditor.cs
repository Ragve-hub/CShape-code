using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace OpenQuant.Finam.Design
{
	public class SessionDataTypeEditor : UITypeEditor
	{
		public static Dictionary<string, TransaqSecurity> instruments = new Dictionary<string, TransaqSecurity>();

		public static Dictionary<int, string> markets = new Dictionary<int, string>();

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
				IWindowsFormsEditorService windowsFormsEditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
				if (windowsFormsEditorService != null)
				{
					SessionDataForm sessionDataForm = new SessionDataForm();
					sessionDataForm.Init(instruments, markets);
					windowsFormsEditorService.ShowDialog(sessionDataForm);
				}
				return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
