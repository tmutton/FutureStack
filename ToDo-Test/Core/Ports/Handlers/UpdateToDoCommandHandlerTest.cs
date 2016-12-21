using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;

namespace ToDoTest.Core.Ports.Handlers
{
    public class UpdateToDoCommandHandlerTest
    {
        [Test]
        public void Test_Updating_a_ToDo_Title()
        {

            /*
                Given that I have a command to add a ToDo
                When I handle that command
                Then I should add to the list of Tasks

            */
            const string TODO_TITLE = "test_title";

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "title_writes_to_database")
                .Options;

            var toDoItem = new ToDoItem() { Title = "This title will be changed" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }


            var command = new UpdateToDoCommand(toDoItem.Id, title: TODO_TITLE);
            var handler = new UpdateToDoCommandHandler(options);

            handler.Handle(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(TODO_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(false, context.ToDoItems.Single().Completed);
            }
        }

        [Test]
        public void Test_Updating_a_ToDo_Completed()
        {

            /*
                Given that I have a command to add a ToDo
                When I handle that command
                Then I should add to the list of Tasks

            */

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "completed_writes_to_database")
                .Options;

            var toDoItem = new ToDoItem() { Title = "This title won't be changed" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }


            var command = new UpdateToDoCommand(toDoItem.Id, complete: true);
            var handler = new UpdateToDoCommandHandler(options);

            handler.Handle(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(toDoItem.Title, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
            }
        }

        [Test]
        public void Test_Updating_a_ToDo_Title_and_Completed()
        {

            /*
                Given that I have a command to add a ToDo
                When I handle that command
                Then I should add to the list of Tasks

            */

            const string TODO_TITLE = "test_title";

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "titlecompleted_writes_to_database")
                .Options;

            var toDoItem = new ToDoItem() { Title = "This title will be changed" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }


            var command = new UpdateToDoCommand(toDoItem.Id, title: TODO_TITLE, complete: true);
            var handler = new UpdateToDoCommandHandler(options);

            handler.Handle(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(TODO_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
            }
        }
    }
}