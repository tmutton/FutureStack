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
                Given that I have a command to add a ToDo
                When I handle that command
                Then I should add to the list of Tasks

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
    }
}