using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using StudentApi.Repository.Context;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Student2Controller : ControllerBase
    {
        StudentDbContext _context;
        public Student2Controller(StudentDbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public IActionResult Get()
        {
            if(_context.Students.ToList().Count== 0)
                return NotFound();
            else
            return Ok(_context.Students.ToList());

        }
        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var student =  _context.Students.SingleOrDefault(x => x.Id == id);  
        if(student!=null)
            {
                return Ok(student);
            }
        else
                return NotFound();
        }

        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return Created("/AddStudent", student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var obj = _context.Students.SingleOrDefault(x => x.Id == id.Value);
            if(obj != null)
            {
                _context.Students.Remove(obj);
                _context.SaveChanges();
                return Ok();
            }
            else 
                return NotFound();

        }


        [HttpPut("{id}")]
 
        public IActionResult EditStudent(int? id, Student student)
        {if (!id.HasValue)
                return BadRequest();
            var obj = _context.Students.SingleOrDefault(x => x.Id == id);
            if (obj != null)
            {
                obj.Name = student.Name;
                obj.Marks = student.Marks;

                _context.SaveChanges();
                return Ok();
            }
            else
                return NotFound() ;
        }
    }
}
