using System;
using System.Xml;
using System.Collections;

namespace OpenNETCF.Xml
{
	/// <summary>
	/// Replicate the some  Functionality of Xpath for Compact Framework.
	/// </summary>
	public class XPath
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public XPath()
		{}
		
		
		/// <summary>
		/// Selects child nodes from a parent node satisfying the xpath
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="xpath"></param>
		/// <returns></returns>
		public static XmlNodeList SelectNodes (XmlNode xmlNode, string xpath)
		{

			XmlNode retXmlNode = xmlNode.Clone();
			retXmlNode.RemoveAll();
	
			IEnumerator iEnumerator = xmlNode.GetEnumerator();
			
			while (iEnumerator.MoveNext())
			{	
				XmlNode childXmlNode = (XmlNode)iEnumerator.Current;
				if (childXmlNode.LocalName.ToString() == xpath)
				{
					retXmlNode.AppendChild(childXmlNode.CloneNode(true));
				}
			}
		
			XmlNodeList retXmlNodeList = retXmlNode.ChildNodes;
			return retXmlNodeList; 
			
		}

		/// <summary>
		/// Gets Child Node for a Parent node.
		/// </summary>
		/// <param name="parent">Parent XmlNode</param>
		/// <param name="xpath">xpath expression of the Child XmlNode</param>
		/// <returns>Child XmlNode</returns>
		public static XmlNode GetChildNodeFromParent (XmlNode parent, string xpath)

		{

			XmlNode retNode = null ;
			string xnName="";
                
			if (xpath.Substring(0,2) == "./")
			{
				foreach (XmlNode xn in parent.ChildNodes)
				{
					xnName = xn.LocalName.ToString();
					if (xnName == xpath.Substring(2))
					{
						retNode = xn;
					}
					else
					{
						foreach (XmlNode xnChild in xn.ChildNodes)
						{
							xnName = xnChild.LocalName.ToString();
							if (xnName == xpath.Substring(2))
							{
								retNode = xnChild;
							}
						}
					}
				}
			}
			if (xpath.Substring(0,1) == "@")
			{
				retNode = parent.Attributes.GetNamedItem(xpath.Substring(1));
			}
			return retNode;
		}

	}
}
