using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Payslips.Data
{
    static class Database
    {
        static public void Add(Person person)
        {
            using (MyContext db = new MyContext())
            {
                try
                {
                    db.Add(person);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка добавления");
                }
            }
        }
        static public void AddRange(List<Person> persons)
        {
            using(MyContext db = new MyContext())
            {
                db.AddRange(persons);
                db.SaveChanges();
            }
        }
        static public void Remove(Person person)
        {
            using (MyContext db = new MyContext())
            {
                db.Remove(person);
                db.SaveChanges();
            }
        }
        static public void RemoveRange(List<Person> persons)
        {
            using (MyContext db = new MyContext())
            {
                db.RemoveRange(persons);
                db.SaveChanges();
            }
        }
        static public List<Person> GetPersonal()
        {
            using (MyContext db = new MyContext())
            {
                return db.Persons.ToList();
            }
        }
    }
}
