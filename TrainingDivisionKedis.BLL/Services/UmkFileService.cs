using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.DAL;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class UmkFileService : IUmkFileService
    {
        private readonly IAppDbContextFactory _contextFactory;
        private readonly IMapper _mapper;
        private readonly IFileService<UmkFilesConfiguration> _fileService;

        public UmkFileService(IAppDbContextFactory contextFactory, IMapper mapper,  IFileService<UmkFilesConfiguration> fileService)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<OperationDetails<GetBySubjectAndTermResponse>> GetBySubjectAndTermAsync(GetBySubjectAndTermRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var checkUserRight = await context.RaspredelenieQuery().CheckTeacherBySubjectAndTerm((short)request.UserId, request.SubjectId, request.TermId);
                    if (checkUserRight == null || checkUserRight.Result == false)
                    {
                        checkUserRight = await context.CurriculumQuery().CheckStudentBySubjectAndTerm(request.UserId, request.SubjectId, request.TermId);
                        if (checkUserRight == null || checkUserRight.Result == false)
                        {
                            throw new Exception("Отказ в доступе");
                        }
                    }
                    var umkFiles = await context.UmkFiles.Where(u => u.SubjectId == request.SubjectId && u.TermId == request.TermId && u.Active).ToListAsync();
                    var subjectName = await context.CurriculumQuery().GetNameById(request.SubjectId);
                    if (subjectName == null)
                        throw new Exception("Дисциплина с Id " + request.SubjectId + " не найдена");
                    var response = new GetBySubjectAndTermResponse(umkFiles, subjectName.Name);
                    return OperationDetails<GetBySubjectAndTermResponse>.Success(response);
                }
                catch (Exception ex)
                {
                    return OperationDetails<GetBySubjectAndTermResponse>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<UmkFile>> CreateAsync(CreateRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var checkUserRight = await context.RaspredelenieQuery().CheckTeacherBySubjectAndTerm((short)request.UserId, request.SubjectId, request.TermId);
                    if (checkUserRight == null || checkUserRight.Result == false)
                    {
                        throw new Exception("Отказ в доступе");
                    }
                    var fileName = await _fileService.UploadAsync(request.UmkFile);
                    var fileSize =  Math.Round(request.UmkFile.Length * 0.001,0);

                    var result = await context.UmkFilesQuery().Create(request.Name,request.SubjectId,request.TermId,fileName,fileSize,request.UmkFile.ContentType);
                    return OperationDetails<UmkFile>.Success(result);
                }
                catch (Exception ex)
                {
                    return OperationDetails<UmkFile>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public OperationDetails<FileDto> GetFileById(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var umkFile = context.UmkFiles.Include(u=>u.FileType).FirstOrDefault(u=>u.Id == id && u.Active);
                    if (umkFile == null)
                        throw new Exception("Файл УМК не найден.");
                    var mas = _fileService.GetFileBytes(umkFile.FileName);
                    var dto = new FileDto()
                    {
                        FileBytes = mas,
                        FileType = umkFile.FileType.Type,
                        FileName = umkFile.FileName
                    };
                    return OperationDetails<FileDto>.Success(dto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<FileDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<UmkFile>> UpdateAsync(UpdateRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {                    
                    var umkFile = context.UmkFiles.FirstOrDefault(u => u.Id == request.Id && u.Active);
                    if (umkFile == null)
                        throw new Exception("Файл УМК не найден.");
                    var checkUserRight = await context.RaspredelenieQuery().CheckTeacherBySubjectAndTerm((short)request.UserId, umkFile.SubjectId, umkFile.TermId);
                    if (checkUserRight == null || checkUserRight.Result == false)
                    {
                        throw new Exception("Отказ в доступе");
                    }
                    string fileName = null;
                    string fileType = null;
                    double? fileSize = null;
                    if (request.UmkFile != null)
                    {
                        fileName = await _fileService.UploadAsync(request.UmkFile);
                        fileType = request.UmkFile.ContentType;
                        fileSize = Math.Round(request.UmkFile.Length * 0.001, 0);                       
                    }

                    var result = await context.UmkFilesQuery().Update(request.Id, request.Name, fileName, fileSize, fileType);
                    return OperationDetails<UmkFile>.Success(result);
                }
                catch (Exception ex)
                {
                    return OperationDetails<UmkFile>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<bool>> DeleteAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    await context.UmkFilesQuery().Delete(id);
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public OperationDetails<UmkFile> GetById(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var umkFile = context.UmkFiles.Include(u => u.FileType).FirstOrDefault(u => u.Id == id && u.Active);
                    if (umkFile == null)
                        throw new Exception("Файл УМК не найден.");
                    return OperationDetails<UmkFile>.Success(umkFile);
                }
                catch (Exception ex)
                {
                    return OperationDetails<UmkFile>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
