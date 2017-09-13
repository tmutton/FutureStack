from uuid import uuid4

from model.models import ToDoItem
from tests.base import TestBase
from ports.handlers import ToDoCreated, ToDoCreatedEventHandler


class HandlerTests(TestBase):
    def test_todo_created_handler_saves(self):
        id = uuid4()

        todo_event = ToDoCreated(id, "Learn Python", False)

        handler = ToDoCreatedEventHandler(self.uow)
        handler.handle(todo_event)

        session = self.uow()

        todo = session.query(ToDoItem).filter_by(id=id).first()
        self.assertEqual(id, todo.id)
        self.assertEqual(todo_event.title, todo.title)
        self.assertEqual(todo_event.completed, todo.completed)

        session.close()
