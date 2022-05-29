using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace OpenQuant.Finam.Design
{
	public class ProxyTypeSelectorEditor : ObjectSelectorEditor
	{
		public static Transaq t;

		public List<string> types = new List<string> { "SOCKS4", "SOCKS5", "HTTP-CONNECT" };

		public override bool IsDropDownResizable => true;

		protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
		{
			if (context == null || context.Instance == null)
			{
				return;
			}
			selector.Clear();
			foreach (string type in types)
			{
				selector.AddNode(type, type, null);
			}
		}
	}
}
