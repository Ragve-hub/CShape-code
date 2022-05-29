using System.Windows.Forms;

namespace OpenQuant.Finam.Design
{
	public class ContractViewItem : ListViewItem
	{
		private TransaqSecurity security;

		public ContractViewItem(TransaqSecurity security, string marketName)
			: base(new string[16])
		{
			this.security = security;
			base.SubItems[0].Text = this.security.SecId.ToString();
			base.SubItems[1].Text = this.security.Active.ToString();
			base.SubItems[2].Text = this.security.SecCode;
			base.SubItems[3].Text = this.security.Board;
			base.SubItems[4].Text = marketName;
			base.SubItems[5].Text = this.security.ShortName;
			base.SubItems[6].Text = this.security.Decimals.ToString();
			base.SubItems[7].Text = $"{this.security.MinStep:0.##########}";
			base.SubItems[8].Text = this.security.LotSize.ToString();
			base.SubItems[9].Text = this.security.PointCost.ToString();
			base.SubItems[10].Text = this.security.UseCredit.ToString();
			base.SubItems[11].Text = this.security.ByMarket.ToString();
			base.SubItems[12].Text = this.security.NoSplit.ToString();
			base.SubItems[13].Text = this.security.ImmOrCancel.ToString();
			base.SubItems[14].Text = this.security.CancelBalance.ToString();
			base.SubItems[15].Text = this.security.SecType;
		}
	}
}
