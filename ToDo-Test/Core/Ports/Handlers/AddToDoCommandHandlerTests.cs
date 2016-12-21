using Microsoft.EntityFrameworkCore;
using System.Linq;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;

namespace ToDo
{
    [TestFixture]
    public class AddToDoCommandHandlerFixture
    {
        [Test]
        public void Test_Adding_A_Task()
        {
            /*
                Given that I have a command to add a ToDo
                When I handle that command
                Then I should add to the list of Tasks

            */
            const string TODO_TITLE = "test_title";
            const int ORDER_NUM = 10;

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            var command = new AddToDoCommand(title:TODO_TITLE, completed: true, order: ORDER_NUM) ;
            var handler = new AddToDoCommandHandler(options);

            handler.Handle(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(TODO_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
                Assert.AreEqual(ORDER_NUM,  context.ToDoItems.Single().Order.Value);
            }
        }
    }
}