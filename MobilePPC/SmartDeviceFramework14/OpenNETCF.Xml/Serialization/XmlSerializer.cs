/*=======================================================================================

	OpenNETCF.Xml.Serialization.XmlSerializer
	Copyright © 2003, OpenNETCF.org

	This library is free software; you can redistribute it and/or modify it under 
	the terms of the OpenNETCF.org Shared Source License.

	This library is distributed in the hope that it will be useful, but 
	WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
	FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
	for more details.

	You should have received a copy of the OpenNETCF.org Shared Source License 
	along with this library; if not, email licensing@opennetcf.org to request a copy.

	If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
	email licensing@opennetcf.org.

	For general enquiries, email enquiries@opennetcf.org or visit our website at:
	http://www.opennetcf.org

=======================================================================================*/
using System;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Collections;

// TODO: properties which return collections
// TODO: method return values

namespace OpenNETCF.Xml.Serialization 
{

	/// <summary>
	/// Serializes and deserializes objects into and from XML documents. The OpenNETCF.Xml.Serialization.XmlSerializer enables you to control how objects are encoded into XML. 
	/// </summary>
	public class XmlSerializer 
	{
		Type t;

		/// <summary>
		/// Initializes a new instance of the System.Xml.Serialization.XmlSerializer class that can serialize objects of the specified type into XML documents, and vice versa. 
		/// </summary>
		/// <param name="type">The type of the object that this OpenNETCF.Xml.Serialization.XmlSerializer can serialize.</param>
		public XmlSerializer(Type type) 
		{
			this.t = type;
		}

		/// <summary>
		/// Serializes the specified System.Object and writes the XML document to a file using the specified System.Xml.XmlWriter.
		/// </summary>
		/// <param name="w">The System.xml.XmlWriter used to write the XML document.</param>
		/// <param name="graph">The System.Object to serialize.</param>
		public void Serialize(XmlTextWriter w, object graph) 
		{
			w.WriteStartDocument();

			// Check to [XmlRoot] attributes
			object[] clsAttribs = t.GetCustomAttributes(typeof(XmlRootAttribute), false);

			if(clsAttribs.Length > 0) 
				w.WriteStartElement((clsAttribs[0] as XmlRootAttribute).ElementName);	
			else 
				w.WriteStartElement(graph.GetType().Name);

			SerializeGraph(w, graph);

			w.WriteEndElement();
			w.WriteEndDocument();
			w.Close();
		}

		/// <summary>
		/// Serializes the specified System.Object and writes the XML document to a file using the specified System.IO.TextWriter.
		/// </summary>
		/// <param name="textWriter">The System.IO.TextWriter used to write the XML document.</param>
		/// <param name="graph">The System.Object to serialize.</param>
		public void Serialize(TextWriter textWriter, object graph)
		{
			XmlTextWriter w = new XmlTextWriter(textWriter);
			this.Serialize(w, graph);
		}

		/// <summary>
		/// Serializes the specified System.Object and writes the XML document to a file using the specified System.IO.Stream.
		/// </summary>
		/// <param name="stream">The System.IO.Stream used to write the XML document.</param>
		/// <param name="graph">The System.Object to serialize.</param>
		public void Serialize(Stream stream, object graph)
		{
			XmlTextWriter w = new XmlTextWriter(stream,System.Text.Encoding.ASCII);
			this.Serialize(w, graph);
		}

		/// <summary>
		/// Recursive internal method that loops through the object's properties and builds an XML tree.
		/// </summary>
		/// <param name="graph">The object graph to serialize.</param>
		/// <param name="w">The XmlTextWriter used to build the XML tree.</param>
		internal void SerializeGraph(XmlTextWriter w, object graph) 
		{
			Type t = graph.GetType();

			if(t.BaseType == typeof(System.Collections.CollectionBase))
			{
				#region Process each item in the collection and serialize

				System.Collections.CollectionBase coll = (System.Collections.CollectionBase)graph;
				foreach(object obj in coll)
				{
					SerializeGraph(w,obj);
				}				

				#endregion
			}
			else
			{
				#region Loop through each property

				// Reflect only on the properties of the type that has been passed to us
				PropertyInfo[] ps = t.GetProperties(BindingFlags.Public|BindingFlags.Instance);

				// Loop through each property...
				foreach(PropertyInfo p in ps) 
				{
					// Check the property for [XmlIgnore] and ignore if it's present
					if ( p.IsDefined(typeof(XmlIgnoreAttribute), false) )
						continue;

					//Ignore read-only properties. We cannot deserialize them anyway
					if ( !p.CanWrite || !p.CanRead )
						continue;

					//Ignore static properties
					if ( p.GetGetMethod().IsStatic || p.GetSetMethod().IsStatic )
						continue;
			
					// Check the property for any attributes
					object[] attribs = p.GetCustomAttributes(false);
					object attrib = null;

					// Pull out the first attribute and let's hope to God that it's an XmlAttribute :)
					if(attribs.Length > 0) 
					{
						int i = 0;
						while(i < attribs.Length) 
						{
							if(attribs[i].ToString().StartsWith("System.Xml.Serialization.Xml")) 
							{
								attrib = attribs[i];
								break;
							}
							i++;
						}
					}

					#region Check for Enum
					
					if(p.PropertyType.IsEnum) 
					{
						object o = p.GetValue(graph,null);
						if(attrib == null)
							w.WriteElementString(p.Name, Convert.ToInt32(o).ToString());
						else if(attrib.GetType() == typeof(XmlElementAttribute)) 
							w.WriteElementString((attrib as XmlElementAttribute).ElementName, Convert.ToInt32(o).ToString());
						else if(attrib.GetType() == typeof(XmlAttributeAttribute)) 
							w.WriteAttributeString((attrib as XmlAttributeAttribute).AttributeName, Convert.ToInt32(o).ToString());
						continue;
					}
					
					#endregion

					#region Check for ValueType or String
					
					if(p.PropertyType.IsValueType || p.PropertyType.FullName == "System.String") 
					{
						object o = p.GetValue(graph,null);

						// Is it a structure?
						if ( !p.PropertyType.IsPrimitive )
						{
							w.WriteStartElement(p.Name);
							SerializeGraph(w, o);
							w.WriteEndElement();
							continue;
						}

						if(o != null) 
						{
							if(attrib == null)
								w.WriteElementString(p.Name, o.ToString());
							else if(attrib.GetType() == typeof(XmlElementAttribute)) 
								w.WriteElementString((attrib as XmlElementAttribute).ElementName,o.ToString());
							else if(attrib.GetType() == typeof(XmlAttributeAttribute)) 
								w.WriteAttributeString((attrib as XmlAttributeAttribute).AttributeName, o.ToString());
						}					
						else
						{
							 if(attrib == null)
								w.WriteElementString(p.Name, "");
							else if(attrib.GetType() == typeof(XmlElementAttribute)) 
								w.WriteElementString((attrib as XmlElementAttribute).ElementName,"");
							else if(attrib.GetType() == typeof(XmlAttributeAttribute)) 
								w.WriteAttributeString((attrib as XmlAttributeAttribute).AttributeName, "");
						}
					}
					#endregion

					#region Check for Collection
					
					else if(p.PropertyType.BaseType == typeof(System.Collections.CollectionBase)) 
					{
						bool bCollectionStart = false; // Are we at the beginning of a collection?
						
						System.Collections.CollectionBase col = (System.Collections.CollectionBase)p.GetValue(graph,null);
						foreach(object obj in col) 
						{
							if(!bCollectionStart) 
							{
								if(attrib == null)
									w.WriteStartElement(p.Name);
								else if(attrib.GetType() == typeof(XmlRootAttribute)) 
									w.WriteStartElement((attrib as XmlRootAttribute).ElementName);
								else if(attrib.GetType() == typeof(XmlElementAttribute)) 
									w.WriteStartElement((attrib as XmlElementAttribute).ElementName);									

								bCollectionStart = true;
							}

							// Recursion rocks! Recurse on the collection we found. 
							SerializeGraph(w,obj);
						}
						w.WriteEndElement();
					}
					#endregion

					#region Check for ReferenceType
					
					else if(t.Assembly.GetType(p.PropertyType.FullName,false) != null) 
					{
						object objInstance = p.GetValue(graph,null);
						if(objInstance != null) 
						{
							if(attrib == null)
								w.WriteStartElement(p.Name);
							else if(attrib.GetType() == typeof(XmlRootAttribute)) 
								w.WriteStartElement((attrib as XmlRootAttribute).ElementName);
							else if(attrib.GetType() == typeof(XmlElementAttribute)) 
								w.WriteStartElement((attrib as XmlElementAttribute).ElementName);
							
							// Recursion rocks! Recurse on the reference type we found. 
							SerializeGraph(w,objInstance);
					
							w.WriteEndElement();
						}
						else
						{
							if(attrib == null)
								w.WriteElementString(p.Name, "");
							else if(attrib.GetType() == typeof(XmlRootAttribute)) 
								w.WriteElementString((attrib as XmlRootAttribute).ElementName,"");
							else if(attrib.GetType() == typeof(XmlElementAttribute)) 
								w.WriteAttributeString((attrib as XmlElementAttribute).ElementName, "");
						}
					}
					#endregion
				}

				#endregion
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlReader"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public object Deserialize(XmlTextReader xmlReader, Type type) 
		{
			object obj;
			xmlReader.MoveToContent();
			obj = DeserializeGraph(xmlReader, type);
			return obj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="r"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public object DeserializeGraph(XmlTextReader r, Type t) 
		{
			object o = t.Assembly.CreateInstance(t.FullName);
			string InstanceName = r.Name;
			int i = 0;

			if(o.GetType().BaseType == typeof(System.Collections.CollectionBase))
			{
				o = DeserializeCollection(r, t);
			}

			PropertyInfo[] properties = o.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance);
			
			#region Handle XmlRoot attributes
			if(r.HasAttributes) 
			{
				while(r.MoveToNextAttribute() && i < properties.Length) 
				{
					object[] butes = properties[i].GetCustomAttributes(false);
					object bute = null;
					if(butes.Length > 0)
						bute = butes[0];

					PropertyInfo property = null;

					if(r.Name == (bute as XmlAttributeAttribute).AttributeName) 
					{
						property = properties[i];
					}
					else 
					{
						property = t.GetProperty(r.Name);
					}

					if(property.PropertyType.Name == "String") 
					{
						// We have a string
						r.ReadAttributeValue();
						string s = r.Value;
						if(property.CanWrite)
							property.SetValue(o, s, null);
					}
					i++;
				}
			}
			#endregion

			while(r.Read() && i < properties.Length) 
			{
				if((r.Name == InstanceName) && (r.NodeType == XmlNodeType.EndElement)) 
				{
					break;
				}

				object[] attribs = properties[i].GetCustomAttributes(false);
				object attrib = null;

				if(attribs.Length > 0)
					attrib = attribs[0];		
	
				if(r.HasAttributes) 
				{
					r.MoveToFirstAttribute();
					while(r.HasAttributes) 
					{
						PropertyInfo property = null;

						if(r.Name == (attrib as XmlAttributeAttribute).AttributeName) 
						{
							property = properties[i];
						}
						else 
						{
							property = t.GetProperty(r.Name);
						}

						if(property.PropertyType.Name == "String") 
						{
							// We have a string
							r.ReadAttributeValue();
							string s = r.Value;
							if(property.CanWrite)
								property.SetValue(o, s, null);
						}
						i++;
					}
				}
				else if((r.NodeType == XmlNodeType.Element)) 
				{
					
					if(r.IsEmptyElement) 
					{
						i++;
						continue;
					}

					PropertyInfo property = null;					
					if(attrib == null) 
					{
						property = t.GetProperty(r.Name);
						if(property == null)
							continue;
					}
					else 
					{
						if(attrib.GetType() == typeof(XmlElementAttribute))
						{
							if(r.Name == (attrib as XmlElementAttribute).ElementName)
							{
								property = properties[i]; 	i++;
							}
							else
								continue;
						}
						else if(attrib.GetType() == typeof(XmlAttributeAttribute))
						{
							if(r.Name == (attrib as XmlAttributeAttribute).AttributeName)
							{
								property = properties[i]; 	i++;
							}
							else
								continue;
						}
						else if(attrib.GetType() == typeof(XmlRootAttribute))
						{
							if(r.Name == (attrib as XmlRootAttribute).ElementName)
							{
								property = properties[i]; 	i++;
							}
							else
								continue;
						}
					}
										
					if(property.PropertyType.BaseType == typeof(System.Collections.CollectionBase)) 
					{
						// We have a collection
						property.SetValue(o, DeserializeCollection(r, property.PropertyType),null);
					}
					else if(property.PropertyType.Name == "String") 
					{
						// We have a string
						r.Read();
						string s = r.Value;
						if(property.CanWrite)
							property.SetValue(o, s, null);
					}
					else if(property.PropertyType.IsValueType) 
					{
						// We have a value type
						object oVal = null;
						if ( typeof(IConvertible).IsAssignableFrom(property.PropertyType) )
						{
							r.Read();
							string s = r.Value;
							if ( property.PropertyType.IsEnum )
							{
								try 
								{ 
									oVal = Enum.ToObject(property.PropertyType, Convert.ToInt32(s));
								} 
								catch(InvalidCastException) 
								{
								}
							}
							else
							{

								try { oVal = Convert.ChangeType(s, property.PropertyType, null); } 
								catch(InvalidCastException) 
								{
									try { oVal = Convert.ChangeType(s, property.PropertyType.BaseType, null); }
									catch(InvalidCastException) { continue; }
								}
							}
						}
						else if ( !property.PropertyType.IsPrimitive )
						{
							oVal = DeserializeGraph(r, property.PropertyType);
						}
						if ( oVal != null )
							property.SetValue(o, oVal, null);
					}
					else if(t.Assembly.GetType(property.PropertyType.FullName, false) != null) 
					{
						// We have a type in the current assembly. Recurse.
						object val = DeserializeGraph(r, property.PropertyType);						
						property.SetValue(o,val, null);
					}
					else 
					{
						throw new NotSupportedException("Cannot deserialize to " + property.PropertyType.FullName + ".");
					}
				}
				else if(r.NodeType == XmlNodeType.Text)
				{
					PropertyInfo property = properties[i];
					i++;

					if(property.PropertyType.Name == "String") 
					{
						// We have a string
						//r.Read();
						string s = r.Value;
						if(property.CanWrite)
							property.SetValue(o, s, null);
					}
				}
			}
			return o;
		}

		private MethodInfo GetMethod(string MethodName, Type t) 
		{
			MethodInfo[] methods = t.GetMethods();
			MethodInfo m = null;

			foreach(MethodInfo method in methods) 
			{
				if(method.Name == MethodName) 
				{
					m = method;
					break;
				}
			}

			if(m == null)
				throw new NotSupportedException("Type " + t.FullName + " does not have a " + MethodName + " method.");

			return m;
		}

		private object DeserializeCollection(XmlTextReader r, Type t) 
		{
			object o = t.Assembly.CreateInstance(t.FullName);

			string InstanceName = r.Name;

			MethodInfo addToColl = GetMethod("Add",t);
			//while(r.Read()) 
			{
				if((r.NodeType == XmlNodeType.EndElement) && (r.Name == InstanceName)) 
				{
					//break;
					return o;
				}

				if(r.NodeType == XmlNodeType.Element) 
				{
					string typeName = r.Name;

					//string namePrefix = t.FullName.Replace(t.FullName, "");
					//Type childType = t.Assembly.GetType(t.FullName + "." + r.Name,true);
					Type childType = addToColl.GetParameters()[0].ParameterType;

					object child = DeserializeGraph(r,childType);
					object[] args = { child };
					addToColl.Invoke(o,args);
				}
			}

			return o;

		}
	}
}