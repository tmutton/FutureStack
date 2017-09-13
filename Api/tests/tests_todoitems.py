from uuid import uuid4

from app import db
from app.models import ToDoItem
from tests.base import BaseTestCase


class ToDoItemTests(BaseTestCase):
    def test_todo_item_saves(self):
        """
            Given that I have a todoitem
            When I save it to the database
            Then I should be able to retrieve it
        """

        id = uuid4()
        todo_item = ToDoItem(id, "Finish this work", False)
        db.session.add(todo_item)
        db.session.commit()

        todo = ToDoItem.query.filter_by(id=id).first()
        self.assertEqual(todo.title, todo_item.title)
        self.assertEqual(todo.completed, todo_item.completed)


