using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using StudentApi.Repository.Context;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Student3Controller : ControllerBase
    {
        StudentDbContext _context;
        public Student3Controller(StudentDbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public ActionResult<List<Student>> Get()
        {
            if(_context.Students.ToList().Count== 0)
                return NotFound();
            else
            return _context.Students.ToList();

        }
        [HttpGet("{id}")]
        public ActionResult<bool> GetStudentById(int id)
        {
            var student =  _context.Students.SingleOrDefault(x => x.Id == id);
            if (student != null)
            {
                return Ok(student);
            }
            else
                return false;
        }

        [HttpPost]
        public ActionResult<int> AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            //return Created("/AddStudent", student);
            return 1;
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> DeleteStudent(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var obj = _context.Students.SingleOrDefault(x => x.Id == id.Value);
            if (obj != null)
            {
                _context.Students.Remove(obj);
                _context.SaveChanges();
                return Ok();
            }
            else
                return false;

        }


        [HttpPut("{id}")]
 
        public ActionResult<string> EditStudent(int? id, Student student)
        {if (!id.HasValue)
                return BadRequest();
            var obj = _context.Students.SingleOrDefault(x => x.Id == id);
            if (obj != null)
            {
                obj.Name = student.Name;
                obj.Marks = student.Marks;

                _context.SaveChanges();
                return "updated";
            }
            else
                return "record with this Id not found"; ;
        }
    }
}
