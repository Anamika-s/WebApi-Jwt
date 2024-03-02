using Microsoft.EntityFrameworkCore;
using StudentApi.Models;
using StudentApi.Repository.Context;

namespace StudentApi.Repository.Context
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext()
        {

        }
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }
        public DbSet<Student>  Students { get; set; }
        public DbSet<User> UserList { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<FileDetails> FileDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=ANAMIKA\\SQLSERVER;database=PracticeDatabase1;integrated security=true;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().
                HasData(new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Manager"
                },
                new Role
                {
                    RoleId = 3,
                    RoleName = "Employee"
                });
            modelBuilder.Entity<User>().
                HasData(new User
                {
                    Id = -1,
                    Email = "admin1@gmail.com",
                   Address="a",
                    Password = "password",
                    RoleId = 1,
                }, new User

                {
                    Id = -2,
                    Email = "admin2@gmail.com",
                    Address = "a",
                    Password = "password",
                    RoleId = 2
                }

                );


        }



    }
}

public class FileService  
{
    private readonly StudentDbContext dbContextClass;
    public FileService(StudentDbContext dbContextClass)
    {
        this.dbContextClass = dbContextClass;
    }
    public async Task PostFileAsync(IFormFile fileData, FileType fileType)
    {
        try
        {
            var fileDetails = new FileDetails()
            {
                ID = 0,
                FileName = fileData.FileName,
                FileType = fileType,
            };
            using (var stream = new MemoryStream())
            {
                fileData.CopyTo(stream);
                fileDetails.FileData = stream.ToArray();
            }
            var result = dbContextClass.FileDetails.Add(fileDetails);
            await dbContextClass.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
    {
        try
        {
            foreach (FileUploadModel file in fileData)
            {
                var fileDetails = new FileDetails()
                {
                    ID = 0,
                    FileName = file.FileDetails.FileName,
                    FileType = file.FileType,
                };
                using (var stream = new MemoryStream())
                {
                    file.FileDetails.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();
                }
                var result = dbContextClass.FileDetails.Add(fileDetails);
            }
            await dbContextClass.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task DownloadFileById(int Id)
    {
        try
        {
            var file = dbContextClass.FileDetails.Where(x => x.ID == Id).FirstOrDefaultAsync();
            var content = new System.IO.MemoryStream(file.Result.FileData);
            var path = Path.Combine(
               Directory.GetCurrentDirectory(), "FileDownloaded",
               file.Result.FileName);
            await CopyStream(content, path);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task CopyStream(Stream stream, string downloadPath)
    {
        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream);
        }
    }
}