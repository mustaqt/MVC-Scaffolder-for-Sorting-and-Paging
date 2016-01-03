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
    public class CompaniesController : Controller
    {
		private readonly ICompanyRepository companyRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public CompaniesController() : this(new CompanyRepository())
        {
        }

        public CompaniesController(ICompanyRepository companyRepository)
        {
			this.companyRepository = companyRepository;
        }

        //
        // GET: /Companies/

        public ViewResult Index()
        {
            return View(companyRepository.AllIncluding(company => company.Customers));
        }

        //
        // GET: /Companies/Details/5

        public ViewResult Details(int id)
        {
            return View(companyRepository.Find(id));
        }

        //
        // GET: /Companies/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Companies/Create

        [HttpPost]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid) {
                companyRepository.InsertOrUpdate(company);
                companyRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }
        
        //
        // GET: /Companies/Edit/5
 
        public ActionResult Edit(int id)
        {
             return View(companyRepository.Find(id));
        }

        //
        // POST: /Companies/Edit/5

        [HttpPost]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid) {
                companyRepository.InsertOrUpdate(company);
                companyRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /Companies/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(companyRepository.Find(id));
        }

        //
        // POST: /Companies/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            companyRepository.Delete(id);
            companyRepository.Save();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                companyRepository.Dispose();
            }
            base.Dispose(disposing);
        }


		//SortingAndPaging

	


		public ViewResult PagedList(CompanyViewModel  companyviewmodel)
        {
			IQueryable<Company> query=companyRepository.AllIncluding(company => company.Customers);
			query=query.OrderBy(x=>x.Id);
			companyviewmodel=PagingSorting(companyviewmodel,query);
			
			return View(companyviewmodel);
		
		}

		private CompanyViewModel PagingSorting(CompanyViewModel model,IQueryable<Company> query )
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
						
					case "Customers":
						query = query.OrderBy(x => x.Customers);
						break;
					case "Customers_Desc":
						query = query.OrderByDescending(x => x.Customers);
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
			model.Records=query.ToPagedList<Company>(model.Page,model.PageSize);;
			
			return model;
		
		}

    }
}

