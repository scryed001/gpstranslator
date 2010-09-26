//==========================================================================================
//
//		OpenNETCF.Windows.Forms.PasswordDeriveBytes
//		Copyright (c) 2003, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================

using System;
using System.Collections;

namespace OpenNETCF.Web.Services2.Dime {

    /// <summary>
    /// A collection of DimeAttachment objects.
    /// </summary>
    public class DimeAttachmentCollection : CollectionBase {
        
        public DimeAttachmentCollection() { }

        public DimeAttachment this[int index] {
            get {                    
                return List[index] as DimeAttachment;
            }
        }

        public DimeAttachment this[string id] {
            get {
                foreach (DimeAttachment a in List) {
                    if (a.Id == id)
                        return a;                    
                }       
                return null;
            }
        }
                                              
        public void Add(DimeAttachment attachment) {
            List.Add(attachment);            
        }        

        public void AddRange(ICollection collection) {
            foreach (DimeAttachment a in collection){
                List.Add(a);
            }
        }
        
        public bool Contains(string id) {                
            return IndexOf(id) != -1;
        }
        
        public void CopyTo(DimeAttachment[] attachments, int index) {
            List.CopyTo(attachments, index);
        }        

        public int IndexOf(DimeAttachment attachment) {
            return List.IndexOf(attachment);
        }

        public int IndexOf(string id) {
            int i = 0;
            foreach (DimeAttachment a in List) {
                if (a.Id == id)
                    return i;
                i++;
            }       
            return -1;
        }

        public void Remove(DimeAttachment attachment) {
            List.Remove(attachment);
        }
    }
}