using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using StudentApi.Repository.Context;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Student1Controller : ControllerBase
    {
        StudentDbContext _context;
        public Student1Controller(StudentDbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public List<Student> Get()
        {
            return _context.Students.ToList();

        }
        [HttpGet("{id}")]
        public Student GetStudentById(int id)
        {
            return _context.Students.SingleOrDefault(x => x.Id == id);  
        }

        [HttpPost]
        [Authorize(Roles="Admin,Manager")]
        public void AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void DeleteStudent(int id)
        {
            var obj = _context.Students.SingleOrDefault(x => x.Id == id);
            if(obj != null)
            {
                _context.Students.Remove(obj);
                _context.SaveChanges();
            }
        }


        [HttpPut("{id}")]
        public bool EditStudent(int id, Student student)
        {
            var obj = _context.Students.SingleOrDefault(x => x.Id == id);
            if (obj != null)
            {
                obj.Name = student.Name;
                obj.Marks = student.Marks;

                _context.SaveChanges();
                return true;
            }
            else
                return false;
        }


        [HttpPatch("{id}")]
        public IActionResult EditPartialStudent(int id, JsonPatchDocument<Student> patchDocument)
        {
            if (patchDocument == null || id < 1) return BadRequest();
            var obj = _context.Students.SingleOrDefault(x => x.Id == id);
            if (obj != null)
            {
                var temp = new Student
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    Address = obj.Address,
                    Marks = obj.Marks
                };
                patchDocument.ApplyTo(temp, ModelState);
                if (!ModelState.IsValid) { return BadRequest(); }
                else
                    obj.Name = temp.Name;
                obj.Address = temp.Address;
                obj.Marks = temp.Marks;
                _context.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }
    }
}
