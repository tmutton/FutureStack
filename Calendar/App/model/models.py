from sqlalchemy import Column, Boolean, String
from sqlalchemy.dialects.postgresql.base import UUID
from sqlalchemy.ext.declarative import declarative_base
from uuid import uuid4

Base = declarative_base()


class ToDoItem(Base):
    __tablename__ = 'todo'

    id = Column(UUID(as_uuid=True), primary_key=True, default=uuid4)
    completed = Column(Boolean)
    title = Column(String)

    def __repr__(self):
        return "<ToDo(id='%s', title='%s', completed='%s' >" % (self.id, self.title, self.completed)

