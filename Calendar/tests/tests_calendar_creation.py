from uuid import uuid4
import unittest

from app.models import ToDoItem, make_todo


class CalendarTests(unittest.TestCase):
    def setUp(self):
        pass

    def test_create_a_todo(self):
        id = uuid4()
        item = ToDoItem(id, "A short summary", False)

        todo = make_todo(item)

        feed = todo.to_ical()
        expected_feed = "BEGIN:VTODO\r\nSTATUS:In Progress\r\nSUMMARY:A short summary\r\nUID:{}\r\nEND:VTODO\r\n".format(id)
        expected_feed_bytes = bytes(expected_feed, "utf8")
        self.assertEqual(feed, expected_feed_bytes)

    def tearDown(self):
        pass
