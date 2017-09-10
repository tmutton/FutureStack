from app import db
from app.models import ToDoItem
from tests.base import BaseTestCase


class ToDoItemTests(BaseTestCase):
    def test_todoitem_saves(self):
        """
            Given that I have a todoitem
            When I save it to the database
            Then I should be able to retrieve it
        """

        todo_item = ToDoItem(uuid4(), "Finish this work", False)
        db.session.add(todo_item)
        db.session.commit()

        ToDoItem.query.filter_by
