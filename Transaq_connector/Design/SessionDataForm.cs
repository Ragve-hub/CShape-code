using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OpenQuant.Finam.Design
{
	public class SessionDataForm : Form
	{
		private IContainer components;

		private ListView contractView;

		private ColumnHeader columnHeader1;

		private ColumnHeader columnHeader2;

		private ColumnHeader columnHeader3;

		private ColumnHeader columnHeader4;

		private ColumnHeader columnHeader5;

		private ColumnHeader columnHeader6;

		private ColumnHeader columnHeader7;

		private ColumnHeader columnHeader8;

		private ColumnHeader columnHeader9;

		private ColumnHeader columnHeader10;

		private ColumnHeader columnHeader11;

		private ColumnHeader columnHeader12;

		private ColumnHeader columnHeader13;

		private ColumnHeader columnHeader14;

		private ColumnHeader columnHeader15;

		private StatusStrip statusStrip;

		private ColumnHeader columnHeader16;

		public SessionDataForm()
		{
			InitializeComponent();
		}

		public void Init(Dictionary<string, TransaqSecurity> instruments, Dictionary<int, string> markets)
		{
			contractView.BeginUpdate();
			contractView.Items.Clear();
			foreach (TransaqSecurity value in instruments.Values)
			{
				if (markets.ContainsKey(value.Market))
				{
					contractView.Items.Add(new ContractViewItem(value, markets[value.Market]));
				}
			}
			contractView.EndUpdate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			contractView = new System.Windows.Forms.ListView();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			columnHeader15 = new System.Windows.Forms.ColumnHeader();
			columnHeader2 = new System.Windows.Forms.ColumnHeader();
			columnHeader16 = new System.Windows.Forms.ColumnHeader();
			columnHeader3 = new System.Windows.Forms.ColumnHeader();
			columnHeader4 = new System.Windows.Forms.ColumnHeader();
			columnHeader5 = new System.Windows.Forms.ColumnHeader();
			columnHeader6 = new System.Windows.Forms.ColumnHeader();
			columnHeader7 = new System.Windows.Forms.ColumnHeader();
			columnHeader8 = new System.Windows.Forms.ColumnHeader();
			columnHeader9 = new System.Windows.Forms.ColumnHeader();
			columnHeader10 = new System.Windows.Forms.ColumnHeader();
			columnHeader11 = new System.Windows.Forms.ColumnHeader();
			columnHeader12 = new System.Windows.Forms.ColumnHeader();
			columnHeader13 = new System.Windows.Forms.ColumnHeader();
			columnHeader14 = new System.Windows.Forms.ColumnHeader();
			statusStrip = new System.Windows.Forms.StatusStrip();
			SuspendLayout();
			contractView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[16]
			{
				columnHeader1, columnHeader15, columnHeader2, columnHeader16, columnHeader3, columnHeader4, columnHeader5, columnHeader6, columnHeader7, columnHeader8,
				columnHeader9, columnHeader10, columnHeader11, columnHeader12, columnHeader13, columnHeader14
			});
			contractView.Dock = System.Windows.Forms.DockStyle.Fill;
			contractView.GridLines = true;
			contractView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			contractView.HideSelection = false;
			contractView.Location = new System.Drawing.Point(0, 0);
			contractView.MultiSelect = false;
			contractView.Name = "contractView";
			contractView.ShowGroups = false;
			contractView.ShowItemToolTips = true;
			contractView.Size = new System.Drawing.Size(922, 458);
			contractView.TabIndex = 0;
			contractView.UseCompatibleStateImageBehavior = false;
			contractView.View = System.Windows.Forms.View.Details;
			columnHeader1.Text = "SecId";
			columnHeader1.Width = 40;
			columnHeader15.Text = "Active";
			columnHeader15.Width = 50;
			columnHeader2.Text = "SecCode";
			columnHeader2.Width = 100;
			columnHeader16.Text = "Board";
			columnHeader3.Text = "Market";
			columnHeader3.Width = 50;
			columnHeader4.Text = "ShortName";
			columnHeader4.Width = 105;
			columnHeader5.Text = "Decimals";
			columnHeader5.Width = 55;
			columnHeader6.Text = "MinStep";
			columnHeader6.Width = 55;
			columnHeader7.Text = "LotSize";
			columnHeader7.Width = 50;
			columnHeader8.Text = "PointCost";
			columnHeader9.Text = "UseCredit";
			columnHeader10.Text = "ByMarket";
			columnHeader11.Text = "NoSplit";
			columnHeader11.Width = 50;
			columnHeader12.Text = "ImmOrCancel";
			columnHeader13.Text = "CancelBalance";
			columnHeader14.Text = "SecType";
			statusStrip.Location = new System.Drawing.Point(0, 458);
			statusStrip.Name = "statusStrip";
			statusStrip.Size = new System.Drawing.Size(922, 22);
			statusStrip.TabIndex = 1;
			statusStrip.Text = "statusStrip1";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(922, 480);
			base.Controls.Add(contractView);
			base.Controls.Add(statusStrip);
			base.MinimizeBox = false;
			base.Name = "SessionDataForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Finam Transaq - Session Data";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
