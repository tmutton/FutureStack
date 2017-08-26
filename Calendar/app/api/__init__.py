from flask import Blueprint

calendar_api = Blueprint('calendar_api', __name__)

from . import calendar