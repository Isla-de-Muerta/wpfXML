using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace Document
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		Channel items = new Channel();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_click1(object sender, RoutedEventArgs e)
        {
			Item it = new Item();
			txtBox.Text = ($"{it.Title = "HABR"}\n {it.Description = "Информация об интересном на habrahabr"}\n {it.PubDate = DateTime.Now.ToString()}");
		}

        private void button_Click2(object sender, RoutedEventArgs e)
		{
			Xml_Fail();
		}

		private void Xml_Fail()
		{
			string URLString = "https://habrahabr.ru/rss/interesting/";
			items.Item = new List<Item>();
			try
			{
				using (XmlTextReader reader = new XmlTextReader(URLString))
				{
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(reader);
					XmlElement xRoot = xDoc.DocumentElement;

					foreach (XmlNode xnode in xRoot.ChildNodes)
					{
                        if (xnode.Name == "channel")
                        {
							foreach (XmlNode childnode in xnode.ChildNodes)
							{
								if (childnode.Name == "item")
								{
									Item temp = new Item();

									foreach (XmlNode child in childnode)
									{
                                        if (child.Name == "title")
                                        {
											temp.Title = child.InnerText;
                                        }
										if (child.Name == "link")
										{
											temp.Link = child.InnerText;
										}
										if (child.Name == "description")
										{
											temp.Description = child.InnerText;
										}
										if (child.Name == "pubDate")
										{
											temp.PubDate = child.InnerText;
										}
									}
									items.Item.Add(temp);
								}
							}
						}
					}

					XmlDocument xmlDoc = new XmlDocument();
					XmlNode rootNode = xmlDoc.CreateElement("items");
					xmlDoc.AppendChild(rootNode);

					foreach (Item elem in items.Item)
					{
						XmlNode item = xmlDoc.CreateElement("item");

						XmlNode title = xmlDoc.CreateElement("title");
						title.InnerText = elem.Title;

						XmlNode link = xmlDoc.CreateElement("link");
						link.InnerText = elem.Link;

						XmlNode description = xmlDoc.CreateElement("description");
						description.InnerText = elem.Description;

						XmlNode pubDate = xmlDoc.CreateElement("pubDate");
						pubDate.InnerText = elem.PubDate;

						item.AppendChild(title);
						item.AppendChild(link);
						item.AppendChild(description);
						item.AppendChild(pubDate);
						rootNode.AppendChild(item);
					}
					xmlDoc.Save("habr.txt");
					txtBox.Text = "Готово файл создан";
				}
			}
			catch (Exception e)
			{
                throw new Exception(txtBox.Text = e.Message);
			}
		}
	}

	[XmlRoot(ElementName = "item")]
	public class Item
	{
		[XmlElement(ElementName = "title")]
		public string Title { get; set; }
		[XmlElement(ElementName = "link")]
		public string Link { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "pubDate")]
		public string PubDate { get; set; }
	}

	[XmlRoot(ElementName = "channel")]
	public class Channel
	{
		[XmlElement(ElementName = "item")]
		public List<Item> Item { get; set; }
	}

	[XmlRoot(ElementName = "rss")]
	public class Rss
	{
		[XmlElement(ElementName = "channel")]
		public Channel Channel { get; set; }
	}
}
