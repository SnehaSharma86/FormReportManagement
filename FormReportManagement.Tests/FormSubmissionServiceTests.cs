
using FormReportManagement.Data;
using FormReportManagement.Services;
using FormReportManagement.Repositories;
using FormReportManagement.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;





namespace FormReportManagement.Tests
{
    public class FormSubmissionServiceTests
    {
        [Fact]
       
        public void Create_ShouldStoreCorrectData()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=FormReportDb_Test;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            using var context = new AppDbContext(options);

            var repository = new FormSubmissionRepository(context);
            var service = new FormSubmissionService(repository);

            var form = new FormSubmission
            {
                Name = "Test User",
                Email = "test@example.com",
                Phone = "9999999999"
            };

            // Act
            service.Create(form);

            // Read the record from the database
            var savedForm = context.FormSubmissions
                .First(x => x.Id == form.Id);

            // Assert
            Assert.Equal("Test User", savedForm.Name);
            Assert.Equal("test@example.com", savedForm.Email);
            Assert.Equal("9999999999", savedForm.Phone);
        }

        [Fact]
        public void Update_ShouldUpdateCorrectColumns()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=FormReportDb_Test;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            using var context = new AppDbContext(options);

            var repository = new FormSubmissionRepository(context);
            var service = new FormSubmissionService(repository);

            // Create an existing record
            var form = new FormSubmission
            {
                Name = "Old Name",
                Email = "old@test.com",
                Phone = "1111111111"
            };

            service.Create(form);

            // Change the values
            form.Name = "New Name";
            form.Email = "new@test.com";
            form.Phone = "2222222222";

            // Act
            service.Update(form);

            // Read the record again from the database
            var updatedForm = context.FormSubmissions
                .First(x => x.Id == form.Id);

            // Assert
            Assert.Equal("New Name", updatedForm.Name);
            Assert.Equal("new@test.com", updatedForm.Email);
            Assert.Equal("2222222222", updatedForm.Phone);
        }
    }
}
