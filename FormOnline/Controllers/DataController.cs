using FormOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormOnline.Controllers
{
    public class DataController : Controller
    {

        DataDbContext context = new DataDbContext();

        //
        // GET: /Data/

        public ActionResult Index()
        {
            List<Form> forms = context.Forms.ToList();

            //Tri par ordre alphabétique
            forms = forms.OrderBy(i => i.Title).ToList();

            //On vérifie si la liste de formulaires n'est pas vide
            if (forms != null)
            {
                if (forms.Count > 0)
                {
                    //On lance la fonction pour vérifier les dates de clôtures
                    forms = CheckDate(forms);
                }
            }

            return View(forms);
        }

        /// <summary>
        /// Fonction qui vérifie les dates de cloture
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public List<Form> CheckDate(List<Form> forms)
        {
            if (forms != null)
            {
                if (forms.Count > 0)
                {
                    //Pour chaque formulaire
                    foreach (Form form in forms)
                    {
                        DateTime dateForm = form.ClosingDate;
                        DateTime dateNow = DateTime.Today;

                        int resul = DateTime.Compare(dateNow, dateForm);

                        //La date du formulaire est inférieure à la date d'aujourd'hui
                        //Doncle formulaire doit être cloturer
                        if (resul == 1)
                        {
                            if (form.Closed == null || form.Closed.Length == 0)
                            {
                                using (context)
                                {
                                    form.Closed = "yes";

                                    context.Entry(form).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }


            return forms;
        }

        //
        // GET: /Data/Details/5

        public ActionResult Details(int id)
        {
            Form form = context.Forms.SingleOrDefault(b => b.FormId == id);

            if (form == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(form);
        }

        //
        // GET: /Data/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Data/Create
        [HttpPost]
        public ActionResult Create(Form form)
        {
            if (ModelState.IsValid)
            {
                //On génère le le hash
                Random alea = new Random();

                //On génère l'url
                string url = "Response/GoToForm/";

                form.Hash = alea.Next(1000000000).ToString();
                form.Url = url + form.Hash;

                //Le formulaire n'est pas cloturé puisqu'il vient d'etre créer
                form.Closed = null;

                context.Forms.Add(form);
                context.SaveChanges();
                return RedirectToAction("ShowPath",form);
            }

            return View(form);
        }

        //
        // GET: /Data/Edit/5

        public ActionResult Edit(int id)
        {
            Form form = context.Forms.Single(p => p.FormId == id);
            if (form == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(form);
        }

        //
        // POST: /Data/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Form form)
        {
            if (ModelState.IsValid)
            {
                using (context)
                {
                    Form _form = context.Forms.Single(p => p.FormId == id);

                    _form.Title = form.Title;
                    _form.Description = form.Description;

                    context.Entry(_form).State = EntityState.Modified;
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            return View(form);
        }

        //
        // GET: /Data/Delete/5

        public ActionResult Delete(int id)
        {
            Form form = context.Forms.Single(p => p.FormId == id);
            if (form == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(form);
        }

        //
        // POST: /Data/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Form form)
        {
            //Get form to delete
            Form _form = context.Forms.Single(p => p.FormId == id);

            #region DeleteAnswers
            //Select all asnwers to this form
            List<Answer> answersToDelete = context.Answers.Where(i => i.question.form.FormId == id).ToList();

            //On supprime les réponses de la base
            foreach (Answer answerToDelete in answersToDelete)
            {
                context.Answers.Remove(answerToDelete);
            }
            #endregion

            #region DeleteQuestions

            //Select All questions to this form
            List<Question> questionsToDelete = context.Questions.Where(i => i.FormId == id).ToList();

            //On supprime chaque question
            foreach (Question questionToDelete in questionsToDelete)
            {
                context.Questions.Remove(questionToDelete);
            }

            #endregion

            #region DeleteStats

            //Select All stats to this form
            List<Stat> statsToDelete = context.Stats.Where(i => i.FormId == id).ToList();

            //On supprime chaque stat
            foreach(Stat statToDelete in statsToDelete)
            {
                context.Stats.Remove(statToDelete);
            }

            #endregion

            //Delete form
            context.Forms.Remove(_form);

            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Cloture(int id)
        {
            Form form = context.Forms.Single(p => p.FormId == id);
            if (form == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(form);
        }

        [HttpPost]
        public ActionResult Cloture(int id, Form form)
        {
            if (ModelState.IsValid)
            {
                using (context)
                {
                    Form _form = context.Forms.Single(p => p.FormId == id);

                    _form.Closed = "yes";

                    context.Entry(_form).State = EntityState.Modified;
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            return View(form);
        }

        public ActionResult NotCloture(int id)
        {
            Form form = context.Forms.Single(p => p.FormId == id);
            if (form == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(form);
        }

        [HttpPost]
        public ActionResult NotCloture(int id, Form form)
        {
            if (ModelState.IsValid)
            {
                using (context)
                {
                    Form _form = context.Forms.Single(p => p.FormId == id);

                    _form.Closed = null;

                    context.Entry(_form).State = EntityState.Modified;
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            return View(form);
        }

        //Renvoie vers la page pour afficher le lien pour accéder au formulaire
        public ActionResult ShowPath(Form form)
        {
            return View(form);
        }
    }
}
