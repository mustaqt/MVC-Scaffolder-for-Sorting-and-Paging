using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CustomScaffolder.Models;
using CustomScafollder.Models;

namespace CustomScafollder.Controllers
{   
    public class CustomersController : Controller
    {
		private readonly ICustomerRepository customerRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public CustomersController() : this(new CustomerRepository())
        {
        }

        public CustomersController(ICustomerRepository customerRepository)
        {
			this.customerRepository = customerRepository;
        }

        //
        // GET: /Customers/

        public ViewResult Index()
        {
            return View(customerRepository.All);
        }

        //
        // GET: /Customers/Details/5

        public ViewResult Details(int id)
        {
            return View(customerRepository.Find(id));
        }

        //
        // GET: /Customers/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Customers/Create

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid) {
                customerRepository.InsertOrUpdate(customer);
                customerRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }
        
        //
        // GET: /Customers/Edit/5
 
        public ActionResult Edit(int id)
        {
             return View(customerRepository.Find(id));
        }

        //
        // POST: /Customers/Edit/5

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid) {
                customerRepository.InsertOrUpdate(customer);
                customerRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /Customers/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(customerRepository.Find(id));
        }

        //
        // POST: /Customers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            customerRepository.Delete(id);
            customerRepository.Save();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                customerRepository.Dispose();
            }
            base.Dispose(disposing);
        }


		//SortingAndPaging

	


		public ViewResult PagedList(CustomerViewModel  customerviewmodel)
        {
			IQueryable<Customer> query=customerRepository.All;
			query=query.OrderBy(x=>x.Id);
			customerviewmodel=PagingSorting(customerviewmodel,query);
			
			return View(customerviewmodel);
		
		}

		private CustomerViewModel PagingSorting(CustomerViewModel model,IQueryable<Customer> query )
		{
			model.IdSort=model.SortOrder=="Id"?"Id_Desc":"Id" ;
			model.NameSort=model.SortOrder=="Name"?"Name_Desc":"Name" ;

			switch(model.SortOrder)
				{	
					case "Id":
						query = query.OrderBy(x => x.Id);
						break;
					case "Id_Desc":
						query = query.OrderByDescending(x => x.Id);
						break;
						
					case "Name":
						query = query.OrderBy(x => x.Name);
						break;
					case "Name_Desc":
						query = query.OrderByDescending(x => x.Name);
						break;
						
					case "Company":
						query = query.OrderBy(x => x.Company);
						break;
					case "Company_Desc":
						query = query.OrderByDescending(x => x.Company);
						break;
						
				
				}

			//Filter
			if(model.IdFilter!=null)
			{
				query = query.Where(x => x.Id.ToString()== model.IdFilter);
			}
			if(!string.IsNullOrWhiteSpace(model.NameFilter))
			{
				query = query.Where(x => x.Name.Contains( model.NameFilter));
			}
			
			if (model.Page == 0) model.Page = 1;
			if (model.PageSize == 0) model.PageSize = 10;
			model.Records=query.ToPagedList<Customer>(model.Page,model.PageSize);;
			
			return model;
		
		}

    }
}

