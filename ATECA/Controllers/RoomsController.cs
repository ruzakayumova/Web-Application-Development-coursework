using ATECA.DAL;
using ATECA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace ATECA.Controllers
{
    public class RoomsController : Controller
    {
        public IRoomsRepository roomsRep;
        // GET: Rooms
        public ActionResult Index(string Name, int? Price, string Category, int? page, string sort)
        {
            int pageNumber = page ?? 1;
            int totalCount;
            int pageSize = 3;

            string sortField = "Name";
            bool sortDesc = false; 
            if (!string.IsNullOrWhiteSpace(sort))
            {
                string[] arr = sort.Split('_');
                if (arr?.Length == 2)
                {
                    sortField = arr[0];
                    sortDesc = arr[1] == "DESC";
                }
            }

            var repository = new RoomsRepository();
            var rooms = repository.Filter(Name, Price, Category, pageNumber, pageSize, sortField, sortDesc, out totalCount);
            var pagedList = new StaticPagedList<Rooms>(rooms, pageNumber, pageSize, totalCount);

            ViewBag.NameSort = sort == "Name_ASC" ? "Name_DSC" : "Name_ASC";
            ViewBag.PriceSort = sort == "Price_ASC" ? "Price_DSC" : "Price_ASC";
            ViewBag.NameSort = sort == "Category_ASC" ? "Category_DSC" : "Category_ASC";
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSort = sort;
            
            return View(pagedList);
        }

        // GET: Rooms/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        public ActionResult Create(Rooms room)
        {
            var repository = new RoomsRepository();
            try
            {
                repository.Insert(room);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int id)
        {
            var repository = new RoomsRepository();
            var room = repository.GetById(id);
            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        public ActionResult Edit(Rooms room)
        {
            var repository = new RoomsRepository();
            try
            {
                repository.Update(room);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int id)
        {
            var repository = new RoomsRepository();
            var room = repository.GetById(id);
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var repository = new RoomsRepository();
            try
            {
                repository.Delete(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
