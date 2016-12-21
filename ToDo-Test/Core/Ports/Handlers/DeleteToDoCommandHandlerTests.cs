using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Commands;

namespace ToDo_Test.Core.Ports.Handlers
{
    [TestFixture]
    public class DeleteToDoCommandHandlerTests
    {
        [Test]
        public void Deleting_a_Task()
        {
            /*
              Given that I have a command to delete a ToDo
              When I handle that command
              Then I should remove the Task from the list of Tasks
          */

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            var toDoItem = new ToDoItem() { Title = "Make delete test pass" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }

            var command = new DeleteToDoCommand(toDoItem.Id);
            var handler = new DeleteToDoCommandHandler(options);

            handler.Handle(command);


            using (var context = new ToDoContext(options))
            {
                Assert.IsFalse(context.ToDoItems.Any(t => t.Id == toDoItem.Id));
            }
        }


    }
}