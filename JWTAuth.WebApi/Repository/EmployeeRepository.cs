
using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;

using Microsoft.EntityFrameworkCore;

namespace JWTAuth.WebApi.Repository;
public class EmployeeRepository : IEmployees
{
    private readonly DatabaseContext _dbContext = new();

    public EmployeeRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<Employee> GetEmployeeDetails()
    {
        try
        {
            return _dbContext.Employees.ToList();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public Employee GetEmployeeDetails(int id)
    {
        try
        {
            Employee? employee = _dbContext.Employees.Find(id);
            if (employee is not null)
            {
                return employee;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void AddEmployee(Employee employee)
    {
        try
        {
            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public void UpdateEmployee(Employee employee)
    {
        try
        {
            _dbContext.Entry(employee).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public Employee DeleteEmployee(int id)
    {
        try
        {
            Employee? employee = _dbContext.Employees.Find(id);

            if (employee is not null)
            {
                _dbContext.Employees.Remove(employee);
                _dbContext.SaveChanges();
                return employee;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public bool CheckEmployee(int id)
    {
        return _dbContext.Employees.Any(e => e.EmployeeID == id);
    }
}
