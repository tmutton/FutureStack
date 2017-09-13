import unittest
from uuid import uuid4

from config import config
from model.models import Base
from model.models import ToDoItem
from ports.handlers import ToDoCreated, ToDoCreatedEventHandler
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker


class HandlerTests(unittest.TestCase):
    def setUp(self):
        db_uri = config['testing'].SQLALCHEMY_DATABASE_URI
        self.engine = create_engine(db_uri)
        self.uow = sessionmaker(bind=self.engine)
        Base.metadata.create_all(self.engine)

    def tearDown(self):
        Base.metadata.drop_all(self.engine)

    def test_me(self):
        self.assertEqual('foo'.upper(), 'FOO')

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

if __name__ == '__main__':
    unittest.main()
