import datetime
from uuid import uuid4
import unittest

from icalendar import vDatetime

from app.models import ToDoItem, make_calendar, make_todo


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

    def test_create_a_calendar_with_todos(self):
        calendar = make_calendar()

        id = uuid4()
        item = ToDoItem(id, "A short summary", False)

        todo = make_todo(item)

        calendar.add_component(todo)

        cal_ics = calendar.to_ical()
        expected_date = vDatetime(datetime.datetime.utcnow()).to_ical().decode("utf8")
        expected_ics = "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nPRODID:-//Todo list calendar//ldnug.org.uk\r\nDTSTART;VALUE=DATE-TIME:{}\r\nBEGIN:VTODO\r\nSTATUS:In Progress\r\nSUMMARY:A short summary\r\nUID:{}\r\nEND:VTODO\r\nEND:VCALENDAR\r\n".format(expected_date, id)
        expected_ics_bytes = bytes(expected_ics, "utf8")
        self.assertEqual(cal_ics, expected_ics_bytes)


    def tearDown(self):
        pass
