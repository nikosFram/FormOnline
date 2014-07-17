using FormOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormOnline.Controllers
{
    public class ResponseController : Controller
    {
        DataDbContext context = new DataDbContext();
        //
        // GET: /Response/

        public ActionResult Index()
        {
            List<Form> forms = context.Forms.Where(x => x.Closed == null).ToList();
            return View(forms);
        }

        public ActionResult GoToForm(int id)
        {
            Form form = context.Forms.SingleOrDefault(b => b.Hash == id.ToString());

            if (form == null)
            {
                return RedirectToAction("Error", "Shared");
            }

            string msgError = "";

            #region Traitement Formulaire complet
            //Si le form est cloturé
            if (form.Closed != null)
            {
                msgError = "Le formulaire est clotûré.";
            }
            else
            {
                //Si le form n'a pas de questions
                if (form.Questions == null)
                {
                    msgError = "Le formulaire n'est pas fini";
                }
                else
                {
                    //Si le form n'a pas de questions
                    if (form.Questions.Count <= 0)
                        msgError = "Le formulaire n'est pas fini";
                    else
                    {
                        //Pour chaque question, on vérifie si elle a au moins 2 réponses
                        foreach (Question quest in form.Questions)
                        {
                            if (quest.Answers == null)
                                msgError = "Le formulaire n'est pas fini";
                            else
                                if (quest.Answers.Count <= 0)
                                    msgError = "Le formulaire n'est pas fini";
                        }
                    }
                }
            }
            #endregion

            ViewData["msgError"] = msgError;

            return View(form);
        }

        //
        // GET: /Response/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Response/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Response/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Response/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Response/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Response/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Response/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
