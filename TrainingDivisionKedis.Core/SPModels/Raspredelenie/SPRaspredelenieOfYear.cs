using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrainingDivisionKedis.Core.SPModels.Raspredelenie
{
    public class SPRaspredelenieOfYear
    {
        [Column("Специальность")]
        [Display(Name = "Специальность")]
        public string Speciality { get; set; }
        [Column("Группа")]
        [Display(Name = "Группа")]
        public string Group { get; set; }
        [Column("Подгруппа")]
        [Display(Name = "П/гр.")]
        public byte SubGroup { get; set; }
        [Column("Семестр")]
        [Display(Name = "Сем-р")]
        public byte Term { get; set; }
        [Column("Цикл")]
        [Display(Name = "Цикл")]
        public string Blok { get; set; }
        [Column("Дисциплина")]
        [Display(Name = "Дисциплина")]
        public string Subject { get; set; }
        [Column("Экзам")]
        [Display(Name = "Экзам")]
        public float Exam { get; set; }
        [Column("Конс")]
        [Display(Name = "Конс")]
        public float Kons { get; set; }
        [Column("КР")]
        [Display(Name = "КР")]
        public float KursRab { get; set; }
        [Column("Преподаватель")]
        [Display(Name = "Преподаватель")]
        public string FIO_Short { get; set; }
        public string Year { get; set; }
        [Column("Объединение")]
        [Display(Name = "Объед-е")]
        public bool Obyedinenie { get; set; }
        public int Nom { get; set; }
        [Column("ФИО")]
        public string Fio { get; set; }
        [Column("Вид_зан")]
        [Display(Name = "Вид зан")]
        public string VidZan { get; set; }
        [Column("Часов")]
        [Display(Name = "Часов")]
        public int Hours { get; set; }
        [Column("Expr1")]
        public int Id { get; set; }
        public int SumHours { get; set; }
        public double SumExam { get; set; }
        public double SumKons { get; set; }
        public double SumKR { get; set; }

        [Display(Name = "Всего")]
        public double TotalHours
        {
            get
            {
                return Hours + Exam + Kons + KursRab;
            }
        }
    }
}
