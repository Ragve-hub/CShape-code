using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace OpenQuant.Finam.Design
{
	public class PasswordChangingTypeEditor : UITypeEditor
	{
		public static DllSelector selectedDll;

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
					PasswordChangingForm passwordChangingForm = new PasswordChangingForm(selectedDll);
					windowsFormsEditorService.ShowDialog(passwordChangingForm);
					if (passwordChangingForm.password != "")
					{
						value = passwordChangingForm.password;
					}
				}
				return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
