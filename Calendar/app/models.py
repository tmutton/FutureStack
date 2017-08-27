from sqlalchemy.dialects.postgresql.base import UUID
from uuid import uuid4

from icalendar import Todo

from app import db
db.UUID = UUID


class ToDoItem(db.Model):
    __tablename__ = 'todo'
    id = db.Column(db.UUID(as_uuid=True), primary_key=True, default=uuid4)
    completed = db.Column(db.Boolean())
    order = db.Column(db.Integer())
    title = db.Column(db.String())

    def __init__(self, id: uuid4, title: str = "", completed: bool = False, order: int = 0) -> None:
        self.id = id
        self.title = title
        self.completed = completed
        self.order = order

    def __repr__(self):
        return '<Summary %r Status %r>' % (self.summary, self.status)


def make_todo(item: ToDoItem) -> Todo:
    todo = Todo()
    todo['uid'] = item.id
    todo['summary'] = item.title
    todo['status'] = "Completed" if item.completed else "In Progress"
    return todo


