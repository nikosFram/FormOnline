using FormOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormOnline.Controllers
{
    public class AnswerController : Controller
    {
        DataDbContext context = new DataDbContext();

        //
        // GET: /Answer/

        public ActionResult Index()
        {
            List<Answer> answers = context.Answers.ToList();

            //Tri par ordre alphabétique
            answers = answers.OrderBy(i => i.question.form.Title).ToList();

            return View(answers);
        }

        //Création d'une question en choisissant un form
        public ActionResult CreateWithChoiceQuestion()
        {
            List<Question> questions = context.Questions.ToList();
            return View(questions);
        }

        //
        // GET: /Answer/Details/5

        public ActionResult Details(int id)
        {
            Answer answer = context.Answers.SingleOrDefault(b => b.AnswerId == id);

            if (answer == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(answer);
        }

        //
        // GET: /Answer/Create

        public ActionResult Create(int id)
        {
            Question question = context.Questions.SingleOrDefault(b => b.QuestionId == id);
            ViewBag.QuestLabel = question.QuestionLabel;

            Answer answer = new Answer();
            answer.QuestionId = id;

            return View(answer);
        }

        //
        // POST: /Answer/Create

        [HttpPost]
        public ActionResult Create(Answer answer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Question question = context.Questions.SingleOrDefault(b => b.QuestionId == answer.QuestionId);
                    question.Answers.Add(answer);
                    context.SaveChanges();
                    return RedirectToAction("Details", "Question", new { id = question.QuestionId });
                }
                return View();
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //
        // GET: /Answer/Edit/5

        public ActionResult Edit(int id)
        {
            Answer answer = context.Answers.Single(p => p.AnswerId == id);
            if (answer == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(answer);
        }

        //
        // POST: /Answer/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Answer answer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (context)
                    {
                        Answer _answer = context.Answers.Single(p => p.AnswerId == id);

                        _answer.AnswerLabel = answer.AnswerLabel;

                        context.Entry(_answer).State = EntityState.Modified;
                        context.SaveChanges();

                        return RedirectToAction("Index");
                    }

                }
                return View(answer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //
        // GET: /Answer/Delete/5

        public ActionResult Delete(int id)
        {
            Answer answer = context.Answers.Single(p => p.AnswerId == id);
            if (answer == null)
            {
                RedirectToAction("Error", "Shared", "");
            }
            return View(answer);
        }

        //
        // POST: /Answer/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Answer answer)
        {
            try
            {
                //Get answer to delete
                Answer _answer = context.Answers.Single(p => p.AnswerId == id);

                #region DeleteStats

                //Select All stats to this answer
                List<Stat> statsToDelete = context.Stats.Where(i => i.AnswerId == id).ToList();

                //On supprime chaque stat
                foreach (Stat statToDelete in statsToDelete)
                {
                    context.Stats.Remove(statToDelete);
                }

                #endregion

                //Delete answer
                context.Answers.Remove(_answer);

                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddAnswer(FormCollection collection)
        {
            try
            {
                //Création d'une nouvelle réponse
                Answer answer = new Answer();

                //On récupère le questionId de la réponse
                answer.QuestionId = Convert.ToInt32(Request.Form["listQuestions"].ToString());

                //On retourne la réponse à la vue
                return View(answer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddAnswerValidate(Answer answer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Question question = context.Questions.SingleOrDefault(b => b.QuestionId == answer.QuestionId);
                    question.Answers.Add(answer);
                    context.SaveChanges();
                    return RedirectToAction("Index", "Answer");
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
