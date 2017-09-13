from uuid import UUID, uuid4

from flask_sqlalchemy import SQLAlchemy
from brightside.handler import Handler, Event
from brightside.messaging import BrightsideMessage, BrightsideMessageStore


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
    def __init__(self, id: UUID, title: str, completed:bool) -> None:
        super().__init__()
        self._id = id
        self._title = title
        self._completed = completed

    @property
    def id(self):
        return self._id

    @property
    def title(self):
        return self._title

    @property
    def completed(self):
        return self._completed

    @property
    def order(self):
        return self._order


class ToDoCreatedEventHandler(Handler):
    def __init__(self, db: SQLAlchemy):
        self._db = db

    def handle(self, request: ToDoCreated) -> None:
        pass
