from flask import Blueprint, jsonify

calhelp = Blueprint('help', __name__)


@calhelp.route('/help', methods=['GET'])
def index():
    return jsonify({
        'status': 'success',
        'message': 'pong!'
    })
