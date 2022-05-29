using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace OpenQuant.Finam.Design
{
	public class AccountSelectorEditor : ObjectSelectorEditor
	{
		public static List<string> clients = new List<string>();

		public override bool IsDropDownResizable => true;

		protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
		{
			if (context == null || context.Instance == null)
			{
				return;
			}
			selector.Clear();
			if (clients.Count <= 0)
			{
				return;
			}
			foreach (string client in clients)
			{
				selector.AddNode(client, client, null);
			}
			selector.Sort();
		}
	}
}
