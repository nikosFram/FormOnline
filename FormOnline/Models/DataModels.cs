using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FormOnline.Models
{
    [Table("Forms")] // Table name
    public class Form
    {
        [Key] // Primary key
        public int FormId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ClosingDate { get; set; }

        public string Hash { get; set; }
        public string Url { get; set; }
        public string Closed { get; set; }

        //Collection de questions
        public virtual ICollection<Question> Questions { get; set; }

        //Statistiques pour les formulaires
        public virtual ICollection<Stat> Stats { get; set; }
    }

    [Table("Questions")] // Table name
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        //[ForeignKey("Form")]
        public int FormId { get; set; }
        public string QuestionLabel { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public virtual Form form { get; set; }
    }

    [Table("Answers")] // Table name
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        //[ForeignKey("Question")]
        public int QuestionId { get; set; }
        public string AnswerLabel { get; set; }

        public virtual Question question { get; set; }
    }

    [Table("Stats")] // Table name
    public class Stat
    {
        [Key]
        public int StatId { get; set; }
        
        public virtual int FormId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }

        public virtual Form form { get; set; }
        public virtual Question question { get; set; }
        public virtual Answer answer { get; set; }
    }
}