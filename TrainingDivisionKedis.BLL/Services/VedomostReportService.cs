using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class VedomostReportService : IVedomostReportService
    {
        private readonly IAppDbContextFactory _contextFactory;
        private readonly ReportsConfiguration _reportsConfiguration;
        private List<SPProgressInStudyGetByRaspredelenieAndUser> _vedomost;
        private List<SPProgressInStudyGetTotals> _totals;
        private string _zamDirectorName;

        private static readonly string DOCX_FILE_MIME_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        public VedomostReportService(IAppDbContextFactory contextFactory, IOptions<ReportsConfiguration> reportsConfiguration)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _reportsConfiguration = reportsConfiguration != null ? reportsConfiguration.Value : throw new ArgumentNullException(nameof(reportsConfiguration));
            var path = Path.Combine(reportsConfiguration.Value.BaseDirectory, reportsConfiguration.Value.VedomostTemplate);
            if (!File.Exists(path))
                throw new FileNotFoundException("Директорий \"" + path + "\" не найден.");
        }

        public async Task<OperationDetails<FileDto>> GetReportAsync(int raspredelenieId)
        {
            try
            {
                using (var context = _contextFactory.Create())
                {
                    _vedomost = await context.ProgressInStudyQuery().GetByRaspredelenie(raspredelenieId);
                    _totals = await context.ProgressInStudyQuery().GetTotalsByRaspredelenie(raspredelenieId);

                    if (_vedomost == null || _totals == null || _vedomost.Count < 1 || _totals.Count < 1)
                        throw new Exception("Неполная информации для формирования отчета");

                    var dirNames = context.DirectorNames.Find((byte)1);
                    if (dirNames != null)
                        _zamDirectorName = dirNames.ZamDirector;
                    else
                        _zamDirectorName = "";
                }

                var tempReport = _reportsConfiguration.BaseDirectory + "TempVedomost.docx";
                File.Copy(_reportsConfiguration.BaseDirectory+_reportsConfiguration.VedomostTemplate, tempReport,true);

                var valuesToFill = GetContent();

                using (var outputDocument = new TemplateProcessor(tempReport)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();
                }
;
                var bytes = File.ReadAllBytes(tempReport);
                var dto = new FileDto()
                {
                    FileBytes = bytes,
                    FileType = DOCX_FILE_MIME_TYPE,
                    FileName = "Ведомость_" + _vedomost[0].SubjectName.Trim() + "_" + _vedomost[0].GroupName.Trim() + "_" + _vedomost[0].Term + ".docx"
                };

                File.Delete(tempReport);
                return OperationDetails<FileDto>.Success(dto);

            }
            catch (Exception ex)
            {
                return OperationDetails<FileDto>.Failure(ex.Message, ex.Source);
            }
        }

        private Content GetContent()
        {
            var studentsTable = new TableContent("StudentsTable");
            for (int i = 0; i < _vedomost.Count; i++)
            {
                studentsTable.AddRow(
                            new FieldContent("N", (i + 1).ToString()),
                            new FieldContent("FullName", _vedomost[i].StudentFio),
                            new FieldContent("Code", _vedomost[i].StudentCode),
                            new FieldContent("Mod1", _vedomost[i].Mod1.ToString()),
                            new FieldContent("Mod2", _vedomost[i].Mod2.ToString()),
                            new FieldContent("Itog", _vedomost[i].Itog.ToString()),
                            new FieldContent("Dop", _vedomost[i].Dop.ToString()),
                            new FieldContent("Ball", _vedomost[i].Ball.ToString()),
                            new FieldContent("Prize", _vedomost[i].PrizeName)
               );
            }

            var allStudents = _totals.FirstOrDefault(t => t.Name.Trim() == "всего");
            var absentStudents = _totals.FirstOrDefault(t => t.Name.Trim() == "неявка");
            var aCount = _totals.FirstOrDefault(t => t.Name.Trim() == "отлично");
            var bCount = _totals.FirstOrDefault(t => t.Name.Trim() == "хорошо");
            var cCount = _totals.FirstOrDefault(t => t.Name.Trim() == "удовл.");
            var dCount = _totals.FirstOrDefault(t => t.Name.Trim() == "неуд.");

            var valuesToFill = new Content(
                new FieldContent("Speciality", _vedomost[0].SpecialityName),
                new FieldContent("Group", _vedomost[0].GroupName),
                new FieldContent("Course", ((_vedomost[0].Term + 1) / 2).ToString()),
                new FieldContent("Term", _vedomost[0].Term.ToString()),
                new FieldContent("Subject", _vedomost[0].SubjectName),
                new FieldContent("ControlDate", _vedomost[0].Date.HasValue ? _vedomost[0].Date.GetValueOrDefault().ToShortDateString() : ""),               
                studentsTable,
                new FieldContent("AllStudents", allStudents?.Value.ToString()),
                new FieldContent("AbsentStudents", absentStudents?.Value.ToString()),
                new FieldContent("ACount", aCount?.Value.ToString()),
                new FieldContent("BCount", bCount?.Value.ToString()),
                new FieldContent("CCount", cCount?.Value.ToString()),
                new FieldContent("DCount", dCount?.Value.ToString()),
                new FieldContent("SubDirector", _zamDirectorName)
            );           

            return valuesToFill;
        }
    }
}
