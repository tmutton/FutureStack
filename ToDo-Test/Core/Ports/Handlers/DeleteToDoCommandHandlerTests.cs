using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using paramore.brighter.commandprocessor;
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
        public void Deleting_ToDo_By_Id()
        {
            /*
              Given that I have a ToDo in the database
              When I issue a command to delete the ToDo with that Id
              Then I should remove the ToDo from the database
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

            var command = new DeleteToDoByIdCommand(toDoItem.Id);
            var handler = new DeleteToDoByIdCommandHandler(options);

            handler.Handle(command);


            using (var context = new ToDoContext(options))
            {
                Assert.IsFalse(context.ToDoItems.Any(t => t.Id == toDoItem.Id));
            }
        }

        [Test]
        public void Delete_All_ToDos()
        {
           /*
              Given that I have ToDos in my database
              When I issue a command to delete all of them
              Then I should remove the ToDos from the database
          */

           var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(new ToDoItem() { Title = "Make delete test pass" });
                context.ToDoItems.Add(new ToDoItem() { Title = "Make delete test pass" });
                context.SaveChanges();
            }

            var command = new DeleteAllToDosCommand();
            var handler = new DeleteAllToDosCommandHandler(options);

            handler.Handle(command);


            using (var context = new ToDoContext(options))
            {
                Assert.IsFalse(context.ToDoItems.Any());
            }

        }

    }

    public class DeleteAllToDosCommandHandler : RequestHandler<DeleteAllToDosCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public DeleteAllToDosCommandHandler(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }
    }

    public class DeleteAllToDosCommand : Command
    {
        public DeleteAllToDosCommand() : base(Guid.NewGuid())
        {
        }
    }


}