from uuid import uuid4

from model import ToDoItem
from tests.base import BaseTestCase
from ports.handlers import ToDoCreated, ToDoCreatedEventHandler


class HandlerTests(BaseTestCase):
    def test_todo_created_handler_saves(self):
        id = uuid4()

        todo_event = ToDoCreated(id, "Learn Python", False)

        handler = ToDoCreatedEventHandler(db)
        handler.handle(todo_event)

        todo = ToDoItem.query.filter_by(id = id).first()
        self.assertEqual(id, todo.id)
        self.assertEqual(todo_event.title, todo.title)
        self.assertEqual(todo_event.completed, todo.completed)
