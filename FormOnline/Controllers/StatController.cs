using FormOnline.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;

namespace FormOnline.Controllers
{
    public class StatController : Controller
    {
        DataDbContext context = new DataDbContext();

        //
        // GET: /Stat/

        public ActionResult Index()
        {
            List<Stat> stats = context.Stats.ToList();

            //Tri par ordre alphabétique
            stats = stats.OrderBy(i => i.form.Title).ToList();

            //Appel fonction
            ArrayList listStats = EnleveDoublonPlusComptage(stats);

            if (listStats == null)
            {
                RedirectToAction("Error", "Shared", "");
            }

            ViewData["listStats"] = listStats;            

            return View(stats);
        }

        /// <summary>
        /// Fonction qui prend en paramètre la liste de toutes les stats de la base et qui renvoie une arraylist avec les stats triées et comptés
        /// </summary>
        /// <param name="stats">Liste des stats de la base</param>
        /// <returns>Arraylist des stats triées et comptées</returns>
        public ArrayList EnleveDoublonPlusComptage(List<Stat> stats)
        {
            ArrayList listStats = new ArrayList();
            bool find = false;

            //Pour chaque stat
            foreach (Stat stat in stats)
            {
                find = false;

                //Pour chaque stats deja présentes dans la liste
                foreach (object[] obj in listStats)
                {
                    //Si la stat existe, on incrémente le cpt
                    if (obj[0] == stat.form.Title && obj[1] == stat.question.QuestionLabel && obj[2] == stat.answer.AnswerLabel)
                    {
                        obj[3] = Convert.ToInt32(obj[3]) + 1;
                        find = true;
                        break;
                    }
                }

                //Si la stat n'existe pas dans la liste, on l'ajoute
                if (!find)
                {
                    object[] obj = new object[] { stat.form.Title, stat.question.QuestionLabel, stat.answer.AnswerLabel, 1 };
                    listStats.Add(obj);
                }
            }

            return listStats;
        }

        //
        // GET: /Stat/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Stat/Create

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Stat stat)
        {
            if (ModelState.IsValid)
            {
                context.Stats.Add(stat);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stat);
        }

        //
        // POST: /Stat/Create

        [HttpPost]
        public ActionResult CreateStat(FormCollection collection)
        {
            try
            {
                int cpt = context.Stats.Count();

                ArrayList listKeys = new ArrayList();

                //On récupère les clés (Attribut FormId + les questionId)
                for (int i = 0; i < Request.Form.Keys.Count; i++)
                {
                    //on récupère la clé
                    string key = Request.Form.Keys[i].ToString();

                    //On n'ajoute que les questionId
                    if (key != "FormId")
                        listKeys.Add(key);

                }

                //On récupère le formId
                int formId = Convert.ToInt32(Request.Form["FormId"].ToString());

                //On récupère le formulaire
                Form form = context.Forms.SingleOrDefault(b => b.FormId == formId);

                using (context)
                {
                    //On récupère les quesionsId ainsi que les answerId
                    foreach (string st in listKeys)
                    {
                        Stat stat = new Stat();
                        stat.FormId = formId;
                        stat.QuestionId = Convert.ToInt32(st);
                        stat.AnswerId = Convert.ToInt32(Request.Form[st].ToString());
                        stat.StatId = cpt;
                        cpt++;
                        form.Stats.Add(stat);
                    }

                    context.Entry(form).State = EntityState.Modified;
                    context.SaveChanges();
                }

                // TODO: Add insert logic here                
                return RedirectToAction("Thanks","Stat",form.Title);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }


        //
        // GET: /Stat/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Stat/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //
        // GET: /Stat/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Stat/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Shared", ex.Message);
            }
        }

        //On récupère les statistiques du formulaire passé en paramètre
        public ActionResult StatForm(int id)
        {
            List<Stat> stats = context.Stats.Where(i => i.FormId == id).ToList();

            //On récupère les infos sur le formulaire
            Form form = context.Forms.Find(id);
            ViewData["FormName"] = form.Title;

            return View(stats);
        }

        //On récupère les statistiques de la question passée en paramètre
        public ActionResult StatQuestion(int id)
        {
            List<Stat> stats = context.Stats.Where(i => i.QuestionId == id).ToList();

            //On récupère les infos sur la question
            Question question = context.Questions.Find(id);
            ViewData["Question"] = question;

            //
            ArrayList listStatQuestion = new ArrayList();

            foreach (var answer in question.Answers)
            {
                object[] obj = new object[] { answer.AnswerLabel, 0 };
                listStatQuestion.Add(obj);
            }

            foreach (var stat in stats)
            {
                foreach (object[] obj in listStatQuestion)
                {
                    if (obj[0] == stat.answer.AnswerLabel)
                    {
                        obj[1] = Convert.ToInt32(obj[1]) + 1;
                        break;
                    }
                }
            }

            ViewData["listStatQuestion"] = listStatQuestion;

            return View(stats);
        }

        //Export PDF des statistiques
        public ActionResult PDF()
        {
            List<Stat> stats = context.Stats.OrderBy(i => i.form.Title).ToList();

            //Appel fonction tri stats
            ArrayList listStats = EnleveDoublonPlusComptage(stats);

            //Si erreur, redirection page erreur
            if(listStats == null)
            {
                RedirectToAction("Error", "Shared", "");
            }

            Document myDocument = new Document(PageSize.A4);
            PdfWriter.GetInstance(myDocument, new FileStream("C:\\wamp\\www\\FormOnline\\FormOnline\\docs\\Export.pdf", FileMode.Create));
            myDocument.Open();

            #region Entetes

            myDocument.AddAuthor("FormOnline");
            myDocument.AddCreationDate();
            myDocument.AddTitle("Export PDF des statistiques");

            #endregion

            PdfPTable table = new PdfPTable(4);

            //Entetes table
            PdfPCell cell = new PdfPCell(new Phrase("Export PDF des statistiques"));
            cell.Colspan = 4;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            

            table.AddCell(cell);
            table.AddCell("Formulaire");
            table.AddCell("Question");
            table.AddCell("Réponse");
            table.AddCell("Nb");

            int cpt = 0;

            //Pour chaque stats
            foreach (object[] obj in listStats)
            {
                PdfPCell cellF = new PdfPCell(new Phrase(obj[0].ToString()));
                PdfPCell cellQ = new PdfPCell(new Phrase(obj[1].ToString()));
                PdfPCell cellA = new PdfPCell(new Phrase(obj[2].ToString()));
                PdfPCell cellNb = new PdfPCell(new Phrase(obj[3].ToString()));

                //Effet de style 1 ligne sur 2
                if (cpt % 2 == 0)
                {
                    cellF.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cellQ.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cellA.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cellNb.BackgroundColor = BaseColor.LIGHT_GRAY;
                }

                table.AddCell(cellF);
                table.AddCell(cellQ);
                table.AddCell(cellA);
                table.AddCell(cellNb);

                cpt++;
            }

            myDocument.Add(table);
            myDocument.Close();

            ViewData["listStats"] = listStats;

            //Retourne à la page index des stats
            return View("Index",stats);
        }

        public ActionResult PDFStatQuestion(int id)
        {
            List<Stat> stats = context.Stats.Where(i => i.QuestionId == id).ToList();

            //On récupère les infos sur la question
            Question question = context.Questions.Find(id);
            ViewData["Question"] = question;

            //
            ArrayList listStatQuestion = new ArrayList();

            foreach (var answer in question.Answers)
            {
                object[] obj = new object[] { answer.AnswerLabel, 0 };
                listStatQuestion.Add(obj);
            }

            foreach (var stat in stats)
            {
                foreach (object[] obj in listStatQuestion)
                {
                    if (obj[0] == stat.answer.AnswerLabel)
                    {
                        obj[1] = Convert.ToInt32(obj[1]) + 1;
                        break;
                    }
                }
            }


            //Affichage PDF
            Document myDocument = new Document(PageSize.A4);
            PdfWriter.GetInstance(myDocument, new FileStream("C:\\wamp\\www\\FormOnline\\FormOnline\\docs\\ExportStatQuestion" +  question.QuestionId + ".pdf", FileMode.Create));
            myDocument.Open();

            #region Entetes

            myDocument.AddAuthor("FormOnline");
            myDocument.AddCreationDate();
            myDocument.AddTitle("Export PDF des statistiques questions");

            #endregion

            PdfPTable table = new PdfPTable(2);

            //Entetes table
            PdfPCell cell = new PdfPCell(new Phrase("Export PDF des statistiques de la question : " + question.QuestionLabel));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;

            PdfPCell cellForm = new PdfPCell(new Phrase("Formulaire : " + question.form.Title));
            cellForm.Colspan = 2;
            cellForm.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            PdfPCell cellQuest = new PdfPCell(new Phrase("Question : " + question.QuestionLabel));
            cellQuest.Colspan = 2;
            cellQuest.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cellQuest.BackgroundColor = BaseColor.LIGHT_GRAY;

            table.AddCell(cell);
            table.AddCell(cellForm);
            table.AddCell(cellQuest);
            table.AddCell("Réponse");
            table.AddCell("Nb");

            int cpt = 0;

            //Pour chaque stats
            foreach (object[] obj in listStatQuestion)
            {
                PdfPCell cellA = new PdfPCell(new Phrase(obj[0].ToString()));
                PdfPCell cellNb = new PdfPCell(new Phrase(obj[1].ToString()));

                //Effet de style 1 ligne sur 2
                if (cpt % 2 == 0)
                {
                    cellA.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cellNb.BackgroundColor = BaseColor.LIGHT_GRAY;
                }
                table.AddCell(cellA);
                table.AddCell(cellNb);

                cpt++;
            }

            myDocument.Add(table);
            myDocument.Close();

            ViewData["listStatQuestion"] = listStatQuestion;

            //Retourne à la page index des stats
            return RedirectToAction("StatQuestion", "Stat", new { id = question.QuestionId });
        }

        //Page de remerciement après avoir répondu au formulaire
        public ActionResult Thanks()
        {
            return View();
        }
    }
}
