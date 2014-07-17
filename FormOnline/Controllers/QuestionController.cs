using FormOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormOnline.Controllers
{
    public class QuestionController : Controller
    {
        DataDbContext context = new DataDbContext();

        //
        // GET: /Question/

        public ActionResult Index()
        {
            List<Question> questions = context.Questions.ToList();

            //Tri par ordre alphabétique
            questions = questions.OrderBy(i => i.form.Title).ToList();

            return View(questions);
        }

        //
        // GET: /Question/Details/5

        public ActionResult Details(int id)
        {
            Question question = context.Questions.SingleOrDefault(b => b.QuestionId == id);

            if (question == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(question);
        }

        //
        // GET: /Question/Create

        public ActionResult Create(int id)
        {
            Form form = context.Forms.SingleOrDefault(b => b.FormId == id);
            ViewBag.FormTitle = form.Title;

            Question question = new Question();
            question.FormId = id;

            return View(question);
        }


        //Création d'une question en choisissant un form
        public ActionResult CreateWithChoiceForm()
        {
            List<Form> forms = context.Forms.ToList();
            return View(forms);
        }

        //
        // POST: /Question/Create

        [HttpPost]
        public ActionResult Create(Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Form form = context.Forms.SingleOrDefault(b => b.FormId == question.FormId);
                    form.Questions.Add(question);
                    context.SaveChanges();
                    return RedirectToAction("Details", "Data", new { id = form.FormId });
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //
        // GET: /Question/Edit/5

        public ActionResult Edit(int id)
        {
            Question question = context.Questions.Single(p => p.QuestionId == id);
            if (question == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(question);
        }

        //
        // POST: /Question/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (context)
                    {
                        Question _question = context.Questions.Single(p => p.QuestionId == id);

                        _question.QuestionLabel = question.QuestionLabel;

                        context.Entry(_question).State = EntityState.Modified;
                        context.SaveChanges();

                        return RedirectToAction("Index");
                    }

                }
                return View(question);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //
        // GET: /Question/Delete/5

        public ActionResult Delete(int id)
        {
            Question question = context.Questions.Single(p => p.QuestionId == id);
            if (question == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(question);
        }

        //
        // POST: /Question/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Question question)
        {
            try
            {
                //get question to delete
                Question _question = context.Questions.Single(p => p.QuestionId == id);

                #region DeleteAnswers
                
                //Select all asnwers to this question
                List<Answer> answersToDelete = context.Answers.Where(i => i.QuestionId == id).ToList();

                //On supprime les réponses de la base
                foreach (Answer answerToDelete in answersToDelete)
                {
                    context.Answers.Remove(answerToDelete);
                }

                #endregion

                #region DeleteStats

                //Select All stats to this question
                List<Stat> statsToDelete = context.Stats.Where(i => i.QuestionId == id).ToList();

                //On supprime chaque stat
                foreach (Stat statToDelete in statsToDelete)
                {
                    context.Stats.Remove(statToDelete);
                }

                #endregion

                //Delete question
                context.Questions.Remove(_question);

                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddQuestion(FormCollection collection)
        {
            try
            {
                //Création d'une nouvelle question
                Question quest = new Question();

                //On récupère le FormId du formulaire
                quest.FormId = Convert.ToInt32(Request.Form["listForms"].ToString());

                //On retourne la question à la vue
                return View(quest);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //Ajout d'une question après avoir choisi le formulaire dans la combo
        [HttpPost]
        public ActionResult AddQuestionValidate(Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Form form = context.Forms.SingleOrDefault(b => b.FormId == question.FormId);
                    form.Questions.Add(question);
                    context.SaveChanges();
                    return RedirectToAction("Index", "Question");
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }
    }
}
