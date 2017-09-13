from flask import Blueprint, make_response

from app.models import ToDoItem, make_calendar, make_todo

calendar_blueprint = Blueprint('calendar_api', __name__)


@calendar_blueprint.route('/calendar', methods=['GET'])
def index():

    calendar = make_calendar()

    todos = ToDoItem.query.all()

    for todo in todos:
        component = make_todo(todo)
        calendar.add_component(component)

    response = make_response(calendar.to_ical())

    response.headers["Content-Disposition"] = "attachment; filename=calendar.ics"
    return response
