from uuid import UUID, uuid4

from brightside.handler import Handler, Event
from brightside.messaging import BrightsideMessage, BrightsideMessageStore
from sqlalchemy.orm import sessionmaker

from model.models import ToDoItem


class FakeMessageStore(BrightsideMessageStore):
    def __init__(self):
        self._message_was_added = None
        self._messages = []

    @property
    def message_was_added(self):
        return self._message_was_added

    def add(self, message: BrightsideMessage):
        self._messages.append(message)
        self._message_was_added = True

    def get_message(self, key):
        for msg in self._messages:
            if msg.id == key:
                return msg
        return None


class ToDoCreated(Event):
    """Note that for interop, we use a field, named in camelCase; we don't want _x field names, because that exposes an
        implementation detail when serialized.  Consumers in other languages may not follow that Python idiom and
        thus hydrate these private fields correctly.
    """
    def __init__(self, id: UUID=None, title: str=None, completed:bool=None) -> None:
        super().__init__()
        self.Id = id
        self.Title = title
        self.Completed = completed


class ToDoCreatedEventHandler(Handler):
    def __init__(self, uow: sessionmaker):
        self._uow = uow

    def handle(self, request: ToDoCreated) -> None:
        session = self._uow()
        try:
            todo = ToDoItem(id=request.Id, title=request.Title, completed=request.Completed)
            session.add(todo)
            session.commit()
        except:
            session.rollback()
            raise
        finally:
            session.close()


