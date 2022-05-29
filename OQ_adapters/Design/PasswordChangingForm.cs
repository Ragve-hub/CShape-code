using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace OpenQuant.Finam.Design
{
	public class PasswordChangingForm : Form
	{
		public DllSelector selectedDll;

		public string password = "";

		private IContainer components;

		private Label label1;

		private TextBox textBox1;

		private TextBox textBox2;

		private Label label2;

		private TextBox textBox3;

		private Label label3;

		private Button button1;

		private Button button2;

		public PasswordChangingForm(DllSelector selectedDll)
		{
			InitializeComponent();
			this.selectedDll = selectedDll;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (textBox2.Text != textBox3.Text)
			{
				MessageBox.Show("Check new password again!!!", "Error");
			}
			else
			{
				string command = "<command id=\"change_pass\" oldpass=\"" + textBox1.Text + "\" newpass=\"" + textBox2.Text + "\"/>";
				try
				{
					TransaqResult transaqResult;
					if (selectedDll == DllSelector.txmlconnector_dll)
					{
						transaqResult = new TransaqResult(MessageParsing(TXmlConnector.Instance.SendCommand(command)));
					}
					else
					{
						if (selectedDll != DllSelector.txcn_dll)
						{
							throw new ArgumentException("Unknown dll " + selectedDll);
						}
						transaqResult = new TransaqResult(MessageParsing(TXcnConnector.Instance.SendCommand(command)));
					}
					if (transaqResult.Success)
					{
						MessageBox.Show("Password change is completed", "Result", MessageBoxButtons.OK);
						password = textBox2.Text;
					}
					else
					{
						MessageBox.Show("Password change is not completed\n\n" + transaqResult.Message, "Result", MessageBoxButtons.OK);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			Close();
		}

		private string MessageParsing(string xmlString)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string text = "";
			text = "";
			xmlDocument.LoadXml(xmlString);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("result");
			if (elementsByTagName.Count > 0)
			{
				for (int i = 0; i < elementsByTagName.Count; i++)
				{
					foreach (XmlAttribute attribute in elementsByTagName[i].Attributes)
					{
						if (!(attribute.Name == "success"))
						{
							continue;
						}
						foreach (XmlAttribute attribute2 in elementsByTagName[i].Attributes)
						{
							string text2 = text;
							text = text2 + attribute2.Name + ";" + attribute2.Value + ";";
						}
						foreach (XmlElement item in elementsByTagName[i])
						{
							string text3 = text;
							text = text3 + item.Name + ";" + item.InnerText + ";";
						}
						return text;
					}
				}
			}
			return "";
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
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
			label1 = new System.Windows.Forms.Label();
			textBox1 = new System.Windows.Forms.TextBox();
			textBox2 = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			textBox3 = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			button1 = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 12);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(78, 13);
			label1.TabIndex = 0;
			label1.Text = "Last password:";
			textBox1.Location = new System.Drawing.Point(135, 12);
			textBox1.Name = "textBox1";
			textBox1.PasswordChar = '*';
			textBox1.Size = new System.Drawing.Size(158, 20);
			textBox1.TabIndex = 2;
			textBox2.Location = new System.Drawing.Point(135, 45);
			textBox2.Name = "textBox2";
			textBox2.PasswordChar = '*';
			textBox2.Size = new System.Drawing.Size(158, 20);
			textBox2.TabIndex = 4;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 48);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(80, 13);
			label2.TabIndex = 3;
			label2.Text = "New password:";
			textBox3.Location = new System.Drawing.Point(135, 80);
			textBox3.Name = "textBox3";
			textBox3.PasswordChar = '*';
			textBox3.Size = new System.Drawing.Size(158, 20);
			textBox3.TabIndex = 6;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 83);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(116, 13);
			label3.TabIndex = 5;
			label3.Text = "Confirm new password:";
			button1.Location = new System.Drawing.Point(15, 123);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(113, 44);
			button1.TabIndex = 7;
			button1.Text = "Change Password";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);
			button2.Location = new System.Drawing.Point(180, 123);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(113, 44);
			button2.TabIndex = 8;
			button2.Text = "Cancel";
			button2.UseVisualStyleBackColor = true;
			button2.Click += new System.EventHandler(button2_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(310, 181);
			base.Controls.Add(button2);
			base.Controls.Add(button1);
			base.Controls.Add(textBox3);
			base.Controls.Add(label3);
			base.Controls.Add(textBox2);
			base.Controls.Add(label2);
			base.Controls.Add(textBox1);
			base.Controls.Add(label1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "PasswordChanging";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Password Changing";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
