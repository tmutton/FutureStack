from flask import Blueprint, jsonify

calendar_blueprint = Blueprint('calendar_api', __name__)


@calendar_blueprint.route('/calendar', methods=['GET'])
def index():
    return jsonify({
        'status': 'success',
        'message': 'pong!'
    })

