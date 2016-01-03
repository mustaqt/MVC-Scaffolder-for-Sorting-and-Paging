
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomScaffolder.Models;
using CustomScafollder.Models;

namespace CustomScafollder.Models


{  
	public class CustomerViewModel
	{
		    		  
   						
		public string IdFilter { get; set; }
		public string IdSort { get; set; }
		  
   						
		public string NameFilter { get; set; }
		public string NameSort { get; set; }
		  
   		
		public int Page { get; set; }
		public int PageSize { get; set; }
		public string SortOrder { get; set; }
		public  PagedList.IPagedList<Customer> Records { get; set; }
   }

}







