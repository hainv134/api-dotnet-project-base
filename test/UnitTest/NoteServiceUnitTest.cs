using Application.Services;
using AutoMapper;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Repository.StaffRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest
{
    public class NoteServiceUnitTest
    {
        private readonly NoteRepository _noteRepo;
        private readonly Mock<AppDbContext> _dbContextMock = new Mock<AppDbContext>();
        private readonly Mock<DbContextOptions<AppDbContext>> optMock = new Mock<DbContextOptions<AppDbContext>>();
        private readonly Mock<AuditableEntitySaveChangesInterceptor> _auditableEntitySaveChangesInterceptorMock = new Mock<AuditableEntitySaveChangesInterceptor>();

        public NoteServiceUnitTest()
        {
            _noteRepo = new NoteRepository(_dbContextMock.Object);
        }

        [Fact]
        public void Test1()
        {

        }
    }
}