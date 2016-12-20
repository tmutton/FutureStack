using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.ViewModelRetrievers;
using ToDoCore.Model;

namespace ToDoTest.Core.Adaptors.ViewModelRetrievers
{
    [TestFixture]
    public class ToDoViewModelRetrieverTests
    {
        [Test]
        public void Test_Retrieveing_A_Task()
        {
            /*
                Given that I have a database with a ToDo
                When I retrieve that todo
                Then I should get a view model for it

            */
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Retrieving_tasks_from_database")
                .Options;

            var toDoItem = new ToDoItem(){Title = "Make test pass"};
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }

            var retriever = new ToDoViewModelRetriever(options);
            var task = retriever.Get(toDoItem.Id);

            Assert.AreEqual(toDoItem.Id, task.Id);
            Assert.AreEqual(toDoItem.Title, task.Title);

        }

        [Test]
        [Ignore("Skip and take not working under EF Core")]
        public void Test_Retrieving_All_Tasks()
        {
            /*
                Given that I have a database with many ToDos
                When I retrieve a a page of those todos
                Then I should get an interable view model for them

            */
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Retrieving_tasks_from_database")
                .Options;

            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.SaveChanges();
            }

            var retriever = new ToDoViewModelRetriever(options);
            var taskList = retriever.Get(1, 3);
            Assert.AreEqual(taskList.Count(), 3);
            taskList = retriever.Get(2, 3);   //only two available on this page
            Assert.AreEqual(taskList.Count(), 2);



        }
    }
}